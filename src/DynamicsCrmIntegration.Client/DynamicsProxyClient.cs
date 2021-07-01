using System;
using System.Net.Http;
using System.Text;
using DynamicsCrmIntegration.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsCrmIntegration.Client
{
    internal class DynamicsProxyClient : IOrganizationService
    {
        private readonly HttpClient _httpClient;

        public DynamicsProxyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private TResponse Send<TRequest, TResponse>(TRequest request)
        {
            var requestMessage = CreateRequest(request);

            var content = AsyncHelpers.RunSync(async () =>
            {
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });

            return Serializer.Deserialize<TResponse>(content);

        }

        private void Send<TRequest>(TRequest request)
        {
            var requestMessage = CreateRequest(request);

            AsyncHelpers.RunSync(async () =>
            {
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
            });
        }

        private HttpRequestMessage CreateRequest<T>(T value)
        {
            var payload = Serializer.Serialize(value);

            return new HttpRequestMessage(HttpMethod.Post, "/")
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            };
        }

        public Guid Create(Entity entity)
        {
            var req = new CreateRequestModel() { Entity = entity };
            var resp = Send<CreateRequestModel, CreateResponseModel>(req);
            return resp.Id;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var req = new RetrieveRequestModel() { EntityName = entityName, Id = id, ColumnSet = columnSet };
            var resp = Send<RetrieveRequestModel, RetrieveResponseModel>(req);
            return resp.Entity;
        }

        public void Update(Entity entity)
        {
            var req = new UpdateModel() { Entity = entity };
            Send(req);
        }

        public void Delete(string entityName, Guid id)
        {
            var req = new DeleteModel() { EntityName = entityName, Id = id };
            Send(req);
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var req = new ExecuteRequestModel() { Request = request };
            var resp = Send<ExecuteRequestModel, ExecuteResponseModel>(req);
            return resp.Response;
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var req = new AssociateModel() { EntityName = entityName, EntityId = entityId, Relationship = relationship, RelatedEntities = relatedEntities };
            Send(req);
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var req = new DisassociateModel() { EntityName = entityName, EntityId = entityId, Relationship = relationship, RelatedEntities = relatedEntities };
            Send(req);
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var req = new RetrieveMultipleRequestModel() { Query = query };
            var resp = Send<RetrieveMultipleRequestModel, RetrieveMultipleResponseModel>(req);
            return resp.EntityCollection;
        }
    }
}
