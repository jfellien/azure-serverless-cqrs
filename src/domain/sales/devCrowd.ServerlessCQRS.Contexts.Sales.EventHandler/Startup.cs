using devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler;
using devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler.Services;
using devCrowd.ServerlessCQRS.Infrastructure.Lib.EventSourcing;
using devCrowd.ServerlessCQRS.ProjectionsStorage;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace devCrowd.ServerlessCQRS.Contexts.Sales.EventHandler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<ProjectionsStorageConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ProjectionsStorage").Bind(settings);
                });
            
            builder.Services.AddSingleton<IStoreProjections, CosmosDbProjectionsStorage>();
            builder.Services.AddSingleton<IHandleEvents, SalesEventHandler>();
        }
    }
}