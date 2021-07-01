using System;

namespace DynamicsCrmIntegration.Models
{
    public class DeleteModel : IRequestModel
    {
        public string EntityName { get; set; }
        public Guid Id { get; set; }
    }
}
