@minLength(3)
@maxLength(24)
param keyVaultName string

resource keyVaultResource 'Microsoft.KeyVault/vaults@2016-10-01' = {
  name: keyVaultName
  location: resourceGroup().location
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    tenantId: tenant().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    createMode: 'default'
    accessPolicies: [
      
    ]
  }
}

output name string = keyVaultResource.name
