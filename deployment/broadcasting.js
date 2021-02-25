"use strict";
const pulumi = require("@pulumi/pulumi");
const azure = require("@pulumi/azure");
const domainContext = require("./domainContext");
const config = new pulumi.Config();
const resourceNames = config.requireObject("resource-names");

const stage = pulumi.getStack().toLowerCase();
const company = resourceNames.company.toLowerCase();
const product = pulumi.getProject().toLowerCase();

exports.build = (resources) => {

    const broadcastingFunctionName = `${company}-${product}-broadcasting-${stage}`;

    const infoQueue = new azure.servicebus.Queue("info-notifications",{
        name:"info-notifications",
        resourceGroupName: resources.rg.name,
        namespaceName: resources.sb.name
    });

    const warningQueue = new azure.servicebus.Queue("warning-notifications",{
        name:"warning-notifications",
        resourceGroupName: resources.rg.name,
        namespaceName: resources.sb.name
    });

    const errorQueue = new azure.servicebus.Queue("error-notifications",{
        name: "error-notifications",
        resourceGroupName: resources.rg.name,
        namespaceName: resources.sb.name
    });

    const functionApp = new azure.appservice.FunctionApp(broadcastingFunctionName,{
        name: broadcastingFunctionName,
        resourceGroupName: resources.rg.name,
        appServicePlanId: resources.servicePlan.id,
        storageAccountName: resources.storage.name,
        storageAccountAccessKey: resources.storage.primaryAccessKey,
        version: "~3",
        appSettings:{
            "APPINSIGHTS_INSTRUMENTATIONKEY": resources.ai.instrumentationKey,
            
            "AzureSignalRConnectionString": resources.signalR.primaryConnectionString,
            
            "COMMUNICATION_HUB_NAME": "broadcast",
            "TARGET_INFO": "newInfo",
            "TARGET_WARNING": "newWarning",
            "TARGET_ERROR": "newError",
            
            "BROADCASTER_CONNECTION_STRING": resources.sb.defaultPrimaryConnectionString,
            "BROADCASTER_INFO_QUEUE": infoQueue.name,
            "BROADCASTER_WARNING_QUEUE": warningQueue.name,
            "BROADCASTER_ERROR_QUEUE": errorQueue.name
        },
        siteConfig:{
            cors:{
                allowedOrigins:['*']
            }
        }
    });

    return {
        connectionString: resources.sb.defaultPrimaryConnectionString,
        infoQueue: infoQueue.name,
        warningQueue: warningQueue.name,
        errorQueue: errorQueue.name
    }
}