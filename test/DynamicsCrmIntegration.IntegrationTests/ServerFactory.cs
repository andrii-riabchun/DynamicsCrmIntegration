using DynamicsCrmIntegration.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicsCrmIntegration.IntegrationTests
{
    public class ServerFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(svc =>
                {
                    var provider = new DynamicsClientProvider();

                    svc.AddScoped(_ => provider.GetCrmOrganizationService());
                });
        }
    }
}
