extern alias dataverse;
using System;
using System.Net.Http;
using dataverse::Microsoft.Xrm.Sdk;
using DynamicsCrmIntegration.Client;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace DynamicsCrmIntegration.IntegrationTests
{
    public class OperationTests :
        IClassFixture<ServerFactory>,
        IClassFixture<DynamicsClientProvider>
    {
        private readonly ServerFactory _serverFactory;
        private readonly DynamicsClientProvider _dynamicsClientProvider;

        public OperationTests(ServerFactory serverFactory, DynamicsClientProvider dynamicsClientProvider)
        {
            _serverFactory = serverFactory;
            _dynamicsClientProvider = dynamicsClientProvider;
        }

        [Fact]
        public void Create()
        {
            var integrationClient = new DynamicsProxyClient(_serverFactory.CreateClient());

            var id = integrationClient.Create(new Entity("sb_quizanswer", Guid.NewGuid()) { ["sb_name"] = "//" });

            var createdEntity = _dynamicsClientProvider.GetCrmOrganizationService().Retrieve("sb_quizanswer", id, new ColumnSet("sb_name"));
            Assert.NotNull(createdEntity);
        }
    }
}
