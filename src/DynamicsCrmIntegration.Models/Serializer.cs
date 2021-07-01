using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DynamicsCrmIntegration.Models
{
    public static class Serializer
    {
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _serializerSettings);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _serializerSettings);
        }

        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            SerializationBinder = new TypeNameBinder
            {
                KnownNamespaces =
                {
                    { typeof(IRequestModel).Namespace, typeof(Serializer).Assembly },
                    { typeof(Entity).Namespace, typeof(Entity).Assembly },
                }
            },
            Converters = {new Converter()}
        };

        class TypeNameBinder : ISerializationBinder
        {
            public Dictionary<string, Assembly> KnownNamespaces { get; } = new Dictionary<string, Assembly>();

            public Type BindToType(string assemblyName, string typeName)
            {
                var assembly = KnownNamespaces.FirstOrDefault(kv => typeName.StartsWith(kv.Key)).Value;
                if (assembly is null)
                    throw new InvalidOperationException($"Can't deserialize '{typeName}'");

                return assembly.GetType(typeName);
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (!KnownNamespaces.Keys.Any(ns => serializedType.Namespace.StartsWith(ns)))
                    throw new InvalidOperationException($"Type '{serializedType.FullName}' has disallowed namespace");

                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }

        class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
                => TryGetKeyValueTypes(objectType, out _, out _);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {

                if (!TryGetKeyValueTypes(objectType, out var keyType, out var valueType))
                    throw new NotSupportedException();

                var result = existingValue ?? Activator.CreateInstance(objectType);

                var add = typeof(DataCollection<,>).MakeGenericType(keyType, valueType)
                   .GetMethod(nameof(DataCollection<int, int>.Add), new[] { keyType, valueType });

                var jObj = JObject.Load(reader);

                if (jObj.TryGetValue("$values", out var values))
                {
                    foreach (var item in (JArray)values)
                    {
                        var kvp = (JObject)item;
                        var key = kvp["$key"].ToObject(keyType);
                        var value = kvp["$value"].ToObject(valueType);
                        add.Invoke(result, new[] { key, value });
                    }
                }

                return result;
            }


            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (!TryGetKeyValueTypes(value.GetType(), out var keyType, out var valueType))
                    throw new NotSupportedException();

                var accessors = GetAccessors(keyType, valueType);

                var obj = new JObject
                {
                    ["$type"] = value.GetType().FullName
                };

                var values = new JArray();
                foreach (var kvp in (IEnumerable)value)
                {
                    values.Add(new JObject()
                    {
                        ["$key"] = JToken.FromObject(accessors.key(kvp)),
                        ["$value"] = JToken.FromObject(accessors.value(kvp)),
                    });
                }

                if (values.Count != 0)
                {
                    obj["$values"] = values;
                }

                obj.WriteTo(writer);
            }

            private static bool TryGetKeyValueTypes(Type objectType, out Type keyType, out Type valueType)
            {
                var baseType = objectType.BaseType;

                keyType = default;
                valueType = default;

                if (baseType is null || !baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(DataCollection<,>))
                    return false;

                var genericArguments = baseType.GetGenericArguments();
                keyType = genericArguments[0];
                valueType = genericArguments[1];

                return true;
            }

            private static (Func<object, object> key, Func<object, object> value) GetAccessors(Type keyType, Type valueType)
            {
                var kvpType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

                var kvpKeyProp = kvpType.GetProperty(nameof(KeyValuePair<int, int>.Key));
                var kvpValueProp = kvpType.GetProperty(nameof(KeyValuePair<int, int>.Value));

                return (
                    kvp => kvpKeyProp.GetValue(kvp),
                    kvp => kvpValueProp.GetValue(kvp)
                );
            }
        }
    }
}
