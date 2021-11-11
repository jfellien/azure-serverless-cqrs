targetScope = 'subscription'

// Setup parameter
// some of them, are mandatory, some of them have default values
@description('The name of the resource group')
param resourceGroupName string

@description('The name of the location (default is germanywestcentral)')
param location string = 'germanywestcentral'

@description('The name of application. This will used for other names')
param applicationName string

@allowed([
  'dev'
  'stage'
  'prod'
])
param environment string = 'dev'

var uniqueName = uniqueString(applicationName, environment, location)

var keyVaultName = 'kv-${uniqueName}'
var appInsightsName = 'ai-${uniqueName}'
var functionAppName = '${applicationName}-${uniqueName}'

// Setup Resource Group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name:  '${resourceGroupName}-${environment}'
  location: location
}

//Setup KeyVault
module keyVault 'templates/keyVault.bicep' = {
  name: 'keyVault-deployment'
  scope: resourceGroup
  params: {
    keyVaultName: keyVaultName
  }
}

//Setup Application Insights
module appInsights 'templates/applicationInsights.bicep' = {
  name: 'appInsights-deployment'
  scope: resourceGroup
  params: {
    appInsightsName: appInsightsName
  }
}

var salesContextName = 'sales'
module salesContext 'templates/setup_context.bicep' = {
  name:'${salesContextName}-deployment'
  scope:resourceGroup
  params:{
    location: location
    contextName: salesContextName
    environment: environment
    keyVaultName: keyVaultName
    appInsightsInstrumentationKey: appInsights.outputs.instrumentationKey
  }
  dependsOn:[
    keyVault
  ]
}
