param appInsightsName string

resource appAppInsightsResource 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  kind:'web'
  location:resourceGroup().location 
  properties:{
    Application_Type:'web'
    Request_Source:'rest'
    Flow_Type:'Bluefield'
  }
}

output instrumentationKey string = reference(appAppInsightsResource.id, '2014-04-01').InstrumentationKey
