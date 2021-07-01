using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DynamicsCrmIntegration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Server
{
    class RequestHandler
    {
        private readonly IOrganizationService _organizationService;

        public RequestHandler(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public IResponseModel Handle(IRequestModel request)
        {
            switch (request)
            {
                case AssociateModel associate:
                    _organizationService.Associate(associate.EntityName, associate.EntityId, associate.Relationship, associate.RelatedEntities);
                    return null;

                case CreateRequestModel create:
                    var id = _organizationService.Create(create.Entity);
                    return new CreateResponseModel() { Id = id };

                case DeleteModel delete:
                    _organizationService.Delete(delete.EntityName, delete.Id);
                    return null;

                case DisassociateModel disassociate:
                    _organizationService.Disassociate(disassociate.EntityName, disassociate.EntityId, disassociate.Relationship, disassociate.RelatedEntities);
                    return null;

                case ExecuteRequestModel execute:
                    var response = _organizationService.Execute(execute.Request);
                    return new ExecuteResponseModel() { Response = response };

                case RetrieveRequestModel retrieve:
                    var entity = _organizationService.Retrieve(retrieve.EntityName, retrieve.Id, retrieve.ColumnSet);
                    return new RetrieveResponseModel() { Entity = entity };

                case RetrieveMultipleRequestModel retrieveMultiple:
                    var collection = _organizationService.RetrieveMultiple(retrieveMultiple.Query);
                    return new RetrieveMultipleResponseModel() { EntityCollection = collection };

                case UpdateModel update:
                    _organizationService.Update(update.Entity);
                    return null;

                default:
                    throw new ArgumentException($"{request.GetType()} is not supported");
            }
        }

        public static async Task Handle(HttpContext context)
        {
            var handler = context.RequestServices.GetRequiredService<RequestHandler>();

            try
            {
                var request = await Read<IRequestModel>(context.Request);

                var result = handler.Handle(request);

                if (result is null)
                    return;

                await Write(context.Response, HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                await Write(context.Response, HttpStatusCode.BadGateway, new ErrorResponse(e));
            }
        }

        private static async Task<T> Read<T>(HttpRequest request)
        {
            var requestContent = await new StreamReader(request.Body).ReadToEndAsync();

            return Serializer.Deserialize<T>(requestContent);
        }

        private static async Task Write<T>(HttpResponse response, HttpStatusCode statusCode, T obj)
        {
            response.StatusCode = (int)statusCode;

            await response.WriteAsync(Serializer.Serialize(obj));
        }
    }
}
