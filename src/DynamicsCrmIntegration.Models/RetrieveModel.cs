using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsCrmIntegration.Models
{
    public class RetrieveRequestModel : IRequestModel
    {
        public string EntityName { get; set; }

        public Guid Id { get; set; }

        public ColumnSet ColumnSet { get; set; }
    }

    public class RetrieveResponseModel : IResponseModel
    {
        public Entity Entity { get; set; }
    }
}
