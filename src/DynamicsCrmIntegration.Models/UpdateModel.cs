using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Models
{
    public class UpdateModel : IRequestModel
    {
        public Entity Entity { get; set; }
    }
}
