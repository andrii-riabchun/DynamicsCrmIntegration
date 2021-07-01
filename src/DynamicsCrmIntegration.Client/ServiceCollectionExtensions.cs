using System;
using System.Data.Common;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicsOnPremiseClient(IServiceCollection services)
        {
            services.AddHttpClient(typeof(DynamicsProxyClient).FullName, (svc, client) =>
            {
                var connectionString = svc.GetRequiredService<IConfiguration>().GetConnectionString("DynamicsConnectionString");
                var builder = new DbConnectionStringBuilder() { ConnectionString = connectionString };
                var url = (string)builder["Url"];
                client.BaseAddress = new Uri(url);
            });

            services.AddTransient<IOrganizationService, DynamicsProxyClient>(svc =>
            {
                var httpClient = svc.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(DynamicsProxyClient).FullName);

                return new DynamicsProxyClient(httpClient);
            });

            return services;
        }
    }
}
