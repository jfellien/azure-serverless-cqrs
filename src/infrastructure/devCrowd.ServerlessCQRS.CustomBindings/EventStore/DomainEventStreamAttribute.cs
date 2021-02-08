using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public class DomainEventStreamAttribute : Attribute
    {
        [AutoResolve] 
        public string ContextName { get; set; }
        
        [AutoResolve] 
        public string EntityName { get; set; }
        
        [AutoResolve] 
        public string EntityId { get; set; }
    }
}