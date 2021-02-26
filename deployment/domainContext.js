"use strict";
const pulumi = require("@pulumi/pulumi");
const azure = require("@pulumi/azure");
const config = new pulumi.Config();
const resourceNames = config.requireObject("resource-names");

const stage = pulumi.getStack().toLowerCase();
const company = resourceNames.company.toLowerCase();
const product = pulumi.getProject().toLowerCase();

exports.build = (contextName, resources) => {

    const commandHandlerName = `${company}-${product}-${contextName}-commandhandler-${stage}`;
    const eventHandlerName = `${company}-${product}-${contextName}-eventhandler-${stage}`;

    const contextEventsTopic = new azure.servicebus.Topic(`${contextName}-events`,{
        name:`${contextName}-events`,
        resourceGroupName: resources.rg.name,
        namespaceName: resources.sb.name
    });

    const contextEventHandlerSubscription = new azure.servicebus.Subscription('context-event-handler',{
        name: 'context-event-handler',
        resourceGroupName: resources.rg.name,
        namespaceName: resources.sb.name,
        topicName: contextEventsTopic.name,
        maxDeliveryCount: 1
    });

    const commandHandler = new azure.appservice.FunctionApp(commandHandlerName,{
        name: commandHandlerName,
        resourceGroupName: resources.rg.name,
        appServicePlanId: resources.servicePlan.id,
        storageAccountName: resources.storage.name,
        storageAccountAccessKey: resources.storage.primaryAccessKey,
        version: "~3",
        appSettings:{
            "APPINSIGHTS_INSTRUMENTATIONKEY": resources.ai.instrumentationKey,
            
            "DOMAIN_CONTEXT": contextName,
            
            "EVENT_STORE_CONNECTION_STRING": resources.db.connectionStrings[0],
            "EVENT_STORE_DB_NAME": resources.eventStore.name,
            "DOMAIN_EVENTS_COLLECTION_NAME":resources.eventsContainer.name,
            
            "EVENT_HANDLER_CONNECTION_STRING": resources.sb.defaultPrimaryConnectionString,
            "EVENT_HANDLER_TOPIC_NAME": contextEventsTopic.name,
            
            "BROADCASTER_CONNECTION_STRING": resources.broadcasting.connectionString,
            "BROADCASTER_INFO_QUEUE": resources.broadcasting.infoQueue,
            "BROADCASTER_WARNING_QUEUE": resources.broadcasting.warningQueue,
            "BROADCASTER_ERROR_QUEUE": resources.broadcasting.errorQueue
        },
        siteConfig:{
            cors:{
                allowedOrigins:['*']
            }
        }
    });

    const eventHandler = new azure.appservice.FunctionApp(eventHandlerName,{
        name: eventHandlerName,
        resourceGroupName: resources.rg.name,
        appServicePlanId: resources.servicePlan.id,
        storageAccountName: resources.storage.name,
        storageAccountAccessKey: resources.storage.primaryAccessKey,
        version: "~3",
        appSettings:{
            "APPINSIGHTS_INSTRUMENTATIONKEY": resources.ai.instrumentationKey,

            "EVENT_HANDLER_CONNECTION_STRING": resources.sb.defaultPrimaryConnectionString,
            "EVENT_HANDLER_TOPIC_NAME": contextEventsTopic.name,
            "EVENT_HANDLER_SUBSCRIPTION_NAME": contextEventHandlerSubscription.name,

            "ProjectionsStorage:ConnectionString": resources.db.connectionStrings[0],
            "ProjectionsStorage:DatabaseName": resources.projections.name,
            "ProjectionsStorage:CollectionName": resources.projectionsContainer.name
        }
    })
}