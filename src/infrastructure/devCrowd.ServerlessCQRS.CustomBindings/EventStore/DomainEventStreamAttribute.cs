using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;

namespace devCrowd.ServerlessCQRS.CustomBindings.EventStore
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public class DomainEventStreamAttribute : Attribute
    {
        public DomainEventStreamAttribute(string contextName) : this(contextName, null, null)
        { }
        
        public DomainEventStreamAttribute(string contextName, string entityName) : this(contextName, entityName, null)
        { }
        
        public DomainEventStreamAttribute(string contextName, string entityName, string entityId)
        {
            ContextName = contextName;
            EntityName = entityName;
            EntityId = entityId;
        }
        
        [AutoResolve] 
        public string ContextName { get; set; }
        
        [AutoResolve] 
        public string EntityName { get; set; }
        
        [AutoResolve] 
        public string EntityId { get; set; }
    }
}