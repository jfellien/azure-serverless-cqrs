param functionAppName string
param storageAccountConnectionSecret string
param appInsightsInstrumentationKey string
@allowed([
  'dotnet'
  'node'
])
param runtime string = 'dotnet'
@allowed([
  '~10'
  '~14'
])
param runtimeVersion string = '~14' 

resource defaultFunctionAppAppsettings 'Microsoft.Web/sites/config@2018-11-01' = {
  name: '${functionAppName}/appsettings'
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appInsightsInstrumentationKey
    AzureWebJobsStorage: storageAccountConnectionSecret
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: runtime
    WEBSITE_ADD_SITENAME_BINDINGS_IN_APPHOST_CONFIG: 1
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionSecret
    WEBSITE_CONTENTSHARE: toLower(functionAppName)
    WEBSITE_NODE_DEFAULT_VERSION: runtimeVersion
  }
}
