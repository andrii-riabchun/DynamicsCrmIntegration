using System;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Models
{
    public class CreateRequestModel : IRequestModel
    {
        public Entity Entity { get; set; }
    }

    public class CreateResponseModel : IResponseModel
    {
        public Guid Id { get; set; }
    }
}
