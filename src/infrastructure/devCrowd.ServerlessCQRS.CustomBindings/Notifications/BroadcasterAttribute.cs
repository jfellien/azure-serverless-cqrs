using System;
using Microsoft.Azure.WebJobs.Description;

namespace devCrowd.ServerlessCQRS.CustomBindings.Notifications
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class BroadcasterAttribute : Attribute
    { }
}