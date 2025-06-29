@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}


param appApiTodoExists bool
param appWebClientExists bool

@description('Id of the user or app to assign application roles')
param principalId string

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

// Monitor application with Azure Monitor
// module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
//   name: 'monitoring'
//   params: {
//     logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
//     applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
//     applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
//     location: location
//     tags: tags
//   }
// }

// later we can use the monitoring module to create Application Insights and a dashboard
// but for now we will just create a Log Analytics workspace
// and use it for Container Apps
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: 'law-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.1.1' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments:[
      {
        principalId: appApiTodoIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
      {
        principalId: appWebClientIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.4.5' = {
  name: 'container-apps-environment'
  params: {
    // logAnalyticsWorkspaceResourceId:  monitoring.outputs.logAnalyticsWorkspaceResourceId
    logAnalyticsWorkspaceResourceId: logAnalytics.id
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false
  }
}

module appApiTodoIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'appApiTodoidentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}appApiTodo-${resourceToken}'
    location: location
  }
}
module appApiTodoFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'appApiTodo-fetch-image'
  params: {
    exists: appApiTodoExists
    name: 'app-api-todo'
  }
}

module appApiTodo 'br/public:avm/res/app/container-app:0.8.0' = {
  name: 'appApiTodo'
  params: {
    name: 'app-api-todo'
    ingressTargetPort: 8080
    scaleMinReplicas: 1
    scaleMaxReplicas: 10
    secrets: {
      secureList:  [
      ]
    }
    containers: [
      {
        image: length(appApiTodoFetchLatestImage.outputs.containers) > 0 ? appApiTodoFetchLatestImage.outputs.containers[0].image : 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: [
          // {
          //   name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          //   value: monitoring.outputs.applicationInsightsConnectionString
          // }
          {
            name: 'AZURE_CLIENT_ID'
            value: appApiTodoIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }
        ]
      }
    ]
    managedIdentities:{
      systemAssigned: false
      userAssignedResourceIds: [appApiTodoIdentity.outputs.resourceId]
    }
    registries:[
      {
        server: containerRegistry.outputs.loginServer
        identity: appApiTodoIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'app-api-todo' })
  }
}

module appWebClientIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'appWebClientidentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}appWebClient-${resourceToken}'
    location: location
  }
}
module appWebClientFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'appWebClient-fetch-image'
  params: {
    exists: appWebClientExists
    name: 'app-web-client'
  }
}

module appWebClient 'br/public:avm/res/app/container-app:0.8.0' = {
  name: 'appWebClient'
  params: {
    name: 'app-web-client'
    ingressTargetPort: 8080
    scaleMinReplicas: 1
    scaleMaxReplicas: 10
    secrets: {
      secureList:  [
      ]
    }
    containers: [
      {
        image: length(appWebClientFetchLatestImage.outputs.containers) > 0 ? appWebClientFetchLatestImage.outputs.containers[0].image : 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: [
          // {
          //   name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          //   value: monitoring.outputs.applicationInsightsConnectionString
          // }
          {
            name: 'AZURE_CLIENT_ID'
            value: appWebClientIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }
        ]
      }
    ]
    managedIdentities:{
      systemAssigned: false
      userAssignedResourceIds: [appWebClientIdentity.outputs.resourceId]
    }
    registries:[
      {
        server: containerRegistry.outputs.loginServer
        identity: appWebClientIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'app-web-client' })
  }
}
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_APP_API_TODO_ID string = appApiTodo.outputs.resourceId
output AZURE_RESOURCE_APP_WEB_CLIENT_ID string = appWebClient.outputs.resourceId
