using devCrowd.ServerlessCQRS.CustomBindings.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(BroadcasterBindingRegistration))]

namespace devCrowd.ServerlessCQRS.CustomBindings.Notifications
{
    public class BroadcasterBindingRegistration : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<BroadcasterBindingConfiguration>();
        }
    }
}