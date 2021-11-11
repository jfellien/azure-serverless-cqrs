param keyVaultName string
param principalId string
@allowed([
  'full'
  'low'
])
param permission string = 'low'

var permissions = {
  full : {
    keys: []
    secrets:[
      'get'
      'list'
      'set'
      'delete'
      'recover'
      'backup'
      'restore'
    ]
  }
  low : {
    keys: []
    secrets:[
      'get'
      'list'
    ]
  }
}

resource keyVaultAccessPolicyResource 'Microsoft.KeyVault/vaults/accessPolicies@2019-09-01' = {
  name: '${keyVaultName}/add'
  properties: {
    accessPolicies: [
      {
        tenantId: tenant().tenantId
        objectId: principalId
        permissions: permissions[permission]
      }
    ]
  }
}
