using devCrowd.ServerlessCQRS.CustomBindings.EventStore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(DomainEventStreamBindingRegistration))]

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    public class DomainEventStreamBindingRegistration : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<DomainEventStreamBindingConfiguration>();
        }
    }
}