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

    const projectionsFunctionName = `${company}-${product}-projections-${stage}`;

    const functionApp = new azure.appservice.FunctionApp(projectionsFunctionName,{
        name: projectionsFunctionName,
        resourceGroupName: resources.rg.name,
        appServicePlanId: resources.servicePlan.id,
        storageAccountName: resources.storage.name,
        storageAccountAccessKey: resources.storage.primaryAccessKey,
        version: "~3",
        appSettings:{
            
            "APPINSIGHTS_INSTRUMENTATIONKEY": resources.ai.instrumentationKey,
            "ProjectionsStorage:ConnectionString": resources.db.connectionStrings[0],
            "ProjectionsStorage:DatabaseName": resources.projections.name,
            "ProjectionsStorage:CollectionName": resources.projectionsContainer.name
            
        },
        siteConfig:{
            cors:{
                allowedOrigins:['*']
            }
        }
    });
}