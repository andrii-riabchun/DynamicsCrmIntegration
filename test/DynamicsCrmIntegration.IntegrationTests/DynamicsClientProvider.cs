extern alias dataverse;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Tooling.Connector;
using DataverseSdk = dataverse::Microsoft.Xrm.Sdk;
using CrmSdk = Microsoft.Xrm.Sdk;

namespace DynamicsCrmIntegration.IntegrationTests
{
    public class DynamicsClientProvider
    {
        private string ConnectionString { get; }

        public DynamicsClientProvider()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<DynamicsClientProvider>()
                .AddEnvironmentVariables()
                .Build();

            ConnectionString = config.GetValue<string>("DynamicsConnectionString") ?? throw new InvalidOperationException("Dynamics connection string in not defined");
        }

        public DataverseSdk.IOrganizationService GetCdsOrganizationService()
        {
            return new ServiceClient(ConnectionString);
        }

        public CrmSdk.IOrganizationService GetCrmOrganizationService()
        {
            return new CrmServiceClient(ConnectionString);
        }
    }
}
