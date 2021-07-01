using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Tooling.Connector;

namespace DynamicsCrmIntegration.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .Build()
               .Run();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
            => services.AddScoped<RequestHandler>();

        public void Configure(IApplicationBuilder app)
            => app.Run(RequestHandler.Handle);
    }
}
