using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Models
{
    public class ExecuteRequestModel : IRequestModel
    {
        public OrganizationRequest Request { get; set; }
    }

    public class ExecuteResponseModel : IResponseModel
    {
        public OrganizationResponse Response { get; set; }
    }
}
