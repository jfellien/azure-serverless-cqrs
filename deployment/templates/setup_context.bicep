param contextName string
param location string
param environment string
param keyVaultName string
param appInsightsInstrumentationKey string

var uniqueName = uniqueString(contextName, location, environment)

var contextStorageAccountName = 'strg${uniqueName}'
var contextAppServicePlanName = 'asp-${uniqueName}'
var commandHandlerFunctionAppName = '${contextName}-commandhandler-${uniqueName}'
var eventHandlerFunctionAppName = '${contextName}-eventhandler-${uniqueName}'

// Setup StorageAccount for Function App
module context_StorageAccount './storageAccount.bicep' = {
  name: 'functionApp-${contextName}-StorageAccount-deployment'
  scope: resourceGroup()
  params: {
    storageAccountName: contextStorageAccountName
  }
}

// Setup App Service Plan
module context_AppServicePlan './consumptionAppServicePlan.bicep' = {
  name: 'functionApp-${contextName}-AppServicePlan-deployment'
  scope: resourceGroup()
  params: {
    appServicePlanName: contextAppServicePlanName
  }
}

// Setup CommandHandler Function App
module commandHandler './functionApp.bicep' = {
  name: '${ contextName }-commandHandler-deployment'
  scope: resourceGroup()
  params: {
    functionAppName: commandHandlerFunctionAppName
    appServicePlanId: context_AppServicePlan.outputs.id
  }
  dependsOn: [
    context_StorageAccount
  ]
}

// Setup CommandHandler Function App
module eventHandler './functionApp.bicep' = {
  name: '${ contextName }-eventHandler-deployment'
  scope: resourceGroup()
  params: {
    functionAppName: eventHandlerFunctionAppName
    appServicePlanId: context_AppServicePlan.outputs.id
  }
  dependsOn: [
    context_StorageAccount
  ]
}

// Setup KeyVault Access Permission for CommandHandler Function App
module commandHandlerKeyVaultAccessPermission './keyVaultAccessPolicy.bicep' = {
  name: 'set-${commandHandlerFunctionAppName}-access-policy'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    principalId: commandHandler.outputs.principalId
    permission: 'low'
  }
  dependsOn:[
    commandHandler
  ]
}

// Setup KeyVault Access Permission for CommandHandler Function App
module eventHandlerKeyVaultAccessPermission './keyVaultAccessPolicy.bicep' = {
  name: 'set-${eventHandlerFunctionAppName}-access-policy'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    principalId: eventHandler.outputs.principalId
    permission: 'low'
  }
  dependsOn:[
    eventHandler
  ]
}

// Setup StorageAccount Connection String Secret
var context_StorageAccount_ConnectionString_Name = '${contextStorageAccountName}-StorageAccount-ConnectionString'
module context_StorageAccount_ConnectionString './keyVaultSecret.bicep' = {
  name: '${context_StorageAccount_ConnectionString_Name}-deployment'
  scope: resourceGroup()
  params: {
    keyVaultName: keyVaultName
    secretName: context_StorageAccount_ConnectionString_Name
    secretValue: context_StorageAccount.outputs.connectionString
  }
}

module commandHandler_AppSettings './functionAppDefaultSettings.bicep' = {
  name: '${commandHandlerFunctionAppName}-AppSettings-deployment'
  scope: resourceGroup()
  params: {
    functionAppName: commandHandlerFunctionAppName
    appInsightsInstrumentationKey: appInsightsInstrumentationKey
    storageAccountConnectionSecret: context_StorageAccount_ConnectionString.outputs.reference
  }
  dependsOn:[
    commandHandler
    commandHandlerKeyVaultAccessPermission
    context_StorageAccount_ConnectionString
  ]
}

module eventHandler_AppSettings './functionAppDefaultSettings.bicep' = {
  name: '${eventHandlerFunctionAppName}-AppSettings-deployment'
  scope: resourceGroup()
  params: {
    functionAppName: eventHandlerFunctionAppName
    appInsightsInstrumentationKey: appInsightsInstrumentationKey
    storageAccountConnectionSecret: context_StorageAccount_ConnectionString.outputs.reference
  }
  dependsOn:[
    eventHandler
    eventHandlerKeyVaultAccessPermission
    context_StorageAccount_ConnectionString
  ]
}
