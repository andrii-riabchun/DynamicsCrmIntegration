using System;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.Models
{
    public class DisassociateModel : IRequestModel
    {
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public Relationship Relationship { get; set; }
        public EntityReferenceCollection RelatedEntities { get; set; }
    }
}
