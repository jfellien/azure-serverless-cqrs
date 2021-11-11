param functionAppName string
param appServicePlanId string

resource functionAppResource 'Microsoft.Web/sites@2020-12-01' = {
  name: functionAppName
  identity:{
    type:'SystemAssigned'
  }
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlanId
  }
}

output principalId string = functionAppResource.identity.principalId
