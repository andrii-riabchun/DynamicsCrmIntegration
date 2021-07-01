using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsCrmIntegration.Models
{
    public class RetrieveMultipleRequestModel : IRequestModel
    {
        public QueryBase Query { get; set; }
    }

    public class RetrieveMultipleResponseModel : IResponseModel
    {
        public EntityCollection EntityCollection { get; set; }
    }
}
