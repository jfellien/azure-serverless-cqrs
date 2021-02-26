using devCrowd.ServerlessCQRS.Projections;
using devCrowd.ServerlessCQRS.ProjectionsStorage;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace devCrowd.ServerlessCQRS.Projections
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
        }
    }
}