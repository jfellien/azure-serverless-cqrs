param appServicePlanName string

resource appServicePlanResource 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServicePlanName
  location: resourceGroup().location
  sku:{
    name: 'Y1'
    tier: 'Dynamic'
  }
}

output id string = appServicePlanResource.id
