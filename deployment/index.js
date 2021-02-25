"use strict";
const pulumi = require("@pulumi/pulumi");
const azure = require("@pulumi/azure");
const domainContext = require("./domainContext");
const projections = require('./projections');
const broadcasting = require("./broadcasting");
const config = new pulumi.Config();
const resourceNames = config.requireObject("resource-names");

const stage = pulumi.getStack().toLowerCase();
const company = resourceNames.company.toLowerCase();
const product = pulumi.getProject().toLowerCase();

const resourceGroupName = `${company}-${product}-${stage}`;
const appInsightsName = `${company}-${product}-ai-${stage}`;
const storageAccountName = `${company}storage${stage}`;

const dbAccountName = `${company}-${product}-dbaccount-${stage}`;

const eventStoreDbName = "event-store";
const eventStoreCollectionName = "domain-events";
const eventStoreCollectionPartitionKeyPath = "/context";

const projectionsDbName = "projections";
const projectionsContainerName = "data";
const projectionsCollectionPartitionKey = "/partitionKey"

const serviceBusNamespaceName = `${company}-${product}-servicebus-${stage}`;

const signalRServiceName = `${company}-signalR-${stage}`;
const staticWebPageUrl = `https://${storageAccountName}.z6.web.core.windows.net`;

const servicePlanName = `${company}-serviceplan-${stage}`;

// Resource Group
const resourceGroup = new azure.core.ResourceGroup(resourceGroupName,{
    name:resourceGroupName
});

// Application Insights
const appInsights = new azure.appinsights.Insights(appInsightsName,{
    name: appInsightsName,
    resourceGroupName: resourceGroup.name,
    applicationType: "web"
})

// Storage Account
const storageAccount = new azure.storage.Account(storageAccountName, {
    name: storageAccountName,
    resourceGroupName: resourceGroup.name,
    accountTier: "Standard",
    accountKind: "StorageV2",
    accountReplicationType: "LRS",
    staticWebsite: {
        indexDocument: "index.html"
    }
});

// CosmosDB Account
const dbAccount = new azure.cosmosdb.Account(dbAccountName,{
    name:dbAccountName,
    resourceGroupName : resourceGroup.name,
    offerType:"Standard",
    kind: "GlobalDocumentDB",
    enableAutomaticFailover:true,
    capabilities:[
        {
            name:"EnableServerless"
        }
    ],
    consistencyPolicy:{
        consistencyLevel:"Eventual"
    },
    geoLocations:[
        {
            failoverPriority: 0,
            location: resourceGroup.location
        }
    ]

});

// EventStore Database
const eventStoreDb = new azure.cosmosdb.SqlDatabase(eventStoreDbName,{
    name:eventStoreDbName,
    resourceGroupName: dbAccount.resourceGroupName,
    accountName: dbAccount.name
})

// EventStore Container
const eventStoreContainer = new azure.cosmosdb.SqlContainer(eventStoreCollectionName,{
    name:eventStoreCollectionName,
    resourceGroupName: dbAccount.resourceGroupName,
    accountName: dbAccount.name,
    databaseName: eventStoreDb.name,
    partitionKeyPath: eventStoreCollectionPartitionKeyPath
})

// Service Bus Namespace
const serviceBus = new azure.servicebus.Namespace(serviceBusNamespaceName,{
    name: serviceBusNamespaceName,
    resourceGroupName: resourceGroup.name,
    sku: "Standard"
})

// SignalR
const signalR = new azure.signalr.Service(signalRServiceName,{
    name:signalRServiceName,
    resourceGroupName: resourceGroup.name,
    sku:{
        name: "Free_F1",
        capacity: 1
    },
    features:[
        {
            flag: "ServiceMode",
            value: "Serverless"
        }
    ],
    cors:[{
        allowedOrigins:['*']
    }]
})

// Service Plan for all Function Apps
const servicePlan = new azure.appservice.Plan(servicePlanName,{
    name: servicePlanName,
    resourceGroupName: resourceGroup.name,
    sku:{
        tier: "ElasticPremium",
        size: "EP1"
    },
    kind: "elastic"
});

// Broadcasting
const broadcastingResources = {
    rg: resourceGroup,
    ai: appInsights,
    storage: storageAccount,
    servicePlan: servicePlan,
    sb: serviceBus,
    signalR: signalR
};

const broadcastService = broadcasting.build(broadcastingResources);

// Projections Database
const projectionsDb = new azure.cosmosdb.SqlDatabase(projectionsDbName,{
    name: projectionsDbName,
    resourceGroupName: dbAccount.resourceGroupName,
    accountName: dbAccount.name
})

// Projections Container
const projectionsContainer = new azure.cosmosdb.SqlContainer(projectionsContainerName,{
    name: projectionsContainerName,
    resourceGroupName: dbAccount.resourceGroupName,
    accountName: dbAccount.name,
    databaseName: projectionsDb.name,
    partitionKeyPath: projectionsCollectionPartitionKey
})

// Projections FunctionApp
const projectionsgResources = {
    rg: resourceGroup,
    ai: appInsights,
    storage: storageAccount,
    servicePlan: servicePlan,
    db: dbAccount,
    projections: projectionsDb,
    projectionsContainer: projectionsContainer
};

const projectionsFunctionsApp = projections.build(projectionsgResources);

// Domain Contexts
const domainContextResources = {
    rg: resourceGroup,
    ai: appInsights,
    storage: storageAccount,
    db: dbAccount,
    eventStore: eventStoreDb,
    eventsContainer: eventStoreContainer,
    sb: serviceBus,
    servicePlan: servicePlan,
    projections: projectionsDb,
    projectionsContainer: projectionsContainer,
    broadcasting: broadcastService
};

const salesContext = domainContext.build("sales", domainContextResources)