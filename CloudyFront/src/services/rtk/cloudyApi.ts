import { api } from './api';

export const addTagTypes = [
  'Authentication',
  'BaseResource',
  'KafkaClusterResource',
  'PostgresServerResource',
  'ResourceGroup',
  'ResourceGroupedResource',
  'ReverseProxy',
  'Status',
  'TenantManagement',
  'VirtualNetworkResource',
  'WebApplicationResource',
] as const;
const injectedRtkApi = api
  .enhanceEndpoints({
    addTagTypes,
  })
  .injectEndpoints({
    endpoints: (build) => ({
      postApiAuthenticationLogin: build.mutation<
        PostApiAuthenticationLoginApiResponse,
        PostApiAuthenticationLoginApiArg
      >({
        query: (queryArg) => ({
          url: `/api/Authentication/Login`,
          method: 'POST',
          body: queryArg.userLoginCommand,
        }),
        invalidatesTags: ['Authentication'],
      }),
      getApiResourceBaseResource: build.query<
        GetApiResourceBaseResourceApiResponse,
        GetApiResourceBaseResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/BaseResource`,
          params: {
            Page: queryArg.page,
            PageSize: queryArg.pageSize,
            FilterBy: queryArg.filterBy,
            OrderBy: queryArg.orderBy,
          },
        }),
        providesTags: ['BaseResource'],
      }),
      getApiResourceBaseResourceDockerEnvironment: build.query<
        GetApiResourceBaseResourceDockerEnvironmentApiResponse,
        GetApiResourceBaseResourceDockerEnvironmentApiArg
      >({
        query: () => ({ url: `/api/resource/BaseResource/dockerEnvironment` }),
        providesTags: ['BaseResource'],
      }),
      getApiResourceBaseResourceByResourceIdContainer: build.query<
        GetApiResourceBaseResourceByResourceIdContainerApiResponse,
        GetApiResourceBaseResourceByResourceIdContainerApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/BaseResource/${queryArg.resourceId}/container`,
        }),
        providesTags: ['BaseResource'],
      }),
      getApiResourceBaseResourceByResourceIdAuditLog: build.query<
        GetApiResourceBaseResourceByResourceIdAuditLogApiResponse,
        GetApiResourceBaseResourceByResourceIdAuditLogApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/BaseResource/${queryArg.resourceId}/audit-log`,
          params: {
            Page: queryArg.page,
            PageSize: queryArg.pageSize,
            FilterBy: queryArg.filterBy,
            OrderBy: queryArg.orderBy,
          },
        }),
        providesTags: ['BaseResource'],
      }),
      postApiResourceBaseResourceByResourceIdPrometheus: build.mutation<
        PostApiResourceBaseResourceByResourceIdPrometheusApiResponse,
        PostApiResourceBaseResourceByResourceIdPrometheusApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/BaseResource/${queryArg.resourceId}/prometheus`,
          method: 'POST',
          body: queryArg.queryPrometheusQuery,
        }),
        invalidatesTags: ['BaseResource'],
      }),
      postApiResourceBaseResourceByResourceIdLoki: build.mutation<
        PostApiResourceBaseResourceByResourceIdLokiApiResponse,
        PostApiResourceBaseResourceByResourceIdLokiApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/BaseResource/${queryArg.resourceId}/loki`,
          method: 'POST',
          body: queryArg.queryLokiQuery,
        }),
        invalidatesTags: ['BaseResource'],
      }),
      getApiResourceKafkaClusterResourceByServerId: build.query<
        GetApiResourceKafkaClusterResourceByServerIdApiResponse,
        GetApiResourceKafkaClusterResourceByServerIdApiArg
      >({
        query: (queryArg) => ({ url: `/api/resource/KafkaClusterResource/${queryArg.serverId}` }),
        providesTags: ['KafkaClusterResource'],
      }),
      deleteApiResourceKafkaClusterResourceByServerId: build.mutation<
        DeleteApiResourceKafkaClusterResourceByServerIdApiResponse,
        DeleteApiResourceKafkaClusterResourceByServerIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['KafkaClusterResource'],
      }),
      getApiResourceKafkaClusterResourceByServerIdTopics: build.query<
        GetApiResourceKafkaClusterResourceByServerIdTopicsApiResponse,
        GetApiResourceKafkaClusterResourceByServerIdTopicsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}/topics`,
        }),
        providesTags: ['KafkaClusterResource'],
      }),
      postApiResourceKafkaClusterResourceByServerIdTopics: build.mutation<
        PostApiResourceKafkaClusterResourceByServerIdTopicsApiResponse,
        PostApiResourceKafkaClusterResourceByServerIdTopicsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}/topics`,
          method: 'POST',
          body: queryArg.createKafkaTopicCommand,
        }),
        invalidatesTags: ['KafkaClusterResource'],
      }),
      postApiResourceKafkaClusterResourceByServerIdTopicsAndTopicId: build.mutation<
        PostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdApiResponse,
        PostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}/topics/${queryArg.topicId}`,
          method: 'POST',
          body: queryArg.produceKafkaTopicMessageCommand,
        }),
        invalidatesTags: ['KafkaClusterResource'],
      }),
      getApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLive: build.query<
        GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiResponse,
        GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}/topics/${queryArg.topicId}/consumeLive`,
        }),
        providesTags: ['KafkaClusterResource'],
      }),
      postApiResourceKafkaClusterResource: build.mutation<
        PostApiResourceKafkaClusterResourceApiResponse,
        PostApiResourceKafkaClusterResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource`,
          method: 'POST',
          body: queryArg.createKafkaClusterCommand,
        }),
        invalidatesTags: ['KafkaClusterResource'],
      }),
      postApiResourceKafkaClusterResourceByServerIdContainerAction: build.mutation<
        PostApiResourceKafkaClusterResourceByServerIdContainerActionApiResponse,
        PostApiResourceKafkaClusterResourceByServerIdContainerActionApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/KafkaClusterResource/${queryArg.serverId}/containerAction`,
          method: 'POST',
          params: {
            actionId: queryArg.actionId,
          },
        }),
        invalidatesTags: ['KafkaClusterResource'],
      }),
      getApiResourcePostgresServerResource: build.query<
        GetApiResourcePostgresServerResourceApiResponse,
        GetApiResourcePostgresServerResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource`,
          params: {
            Page: queryArg.page,
            PageSize: queryArg.pageSize,
            FilterBy: queryArg.filterBy,
            OrderBy: queryArg.orderBy,
          },
        }),
        providesTags: ['PostgresServerResource'],
      }),
      postApiResourcePostgresServerResource: build.mutation<
        PostApiResourcePostgresServerResourceApiResponse,
        PostApiResourcePostgresServerResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource`,
          method: 'POST',
          body: queryArg.createPostgresServerCommand,
        }),
        invalidatesTags: ['PostgresServerResource'],
      }),
      getApiResourcePostgresServerResourceByServerId: build.query<
        GetApiResourcePostgresServerResourceByServerIdApiResponse,
        GetApiResourcePostgresServerResourceByServerIdApiArg
      >({
        query: (queryArg) => ({ url: `/api/resource/PostgresServerResource/${queryArg.serverId}` }),
        providesTags: ['PostgresServerResource'],
      }),
      deleteApiResourcePostgresServerResourceByServerId: build.mutation<
        DeleteApiResourcePostgresServerResourceByServerIdApiResponse,
        DeleteApiResourcePostgresServerResourceByServerIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource/${queryArg.serverId}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['PostgresServerResource'],
      }),
      getApiResourcePostgresServerResourceDatabaseByDatabaseId: build.query<
        GetApiResourcePostgresServerResourceDatabaseByDatabaseIdApiResponse,
        GetApiResourcePostgresServerResourceDatabaseByDatabaseIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource/database/${queryArg.databaseId}`,
        }),
        providesTags: ['PostgresServerResource'],
      }),
      postDatabaseByDatabaseIdRunQuery: build.mutation<
        PostDatabaseByDatabaseIdRunQueryApiResponse,
        PostDatabaseByDatabaseIdRunQueryApiArg
      >({
        query: (queryArg) => ({
          url: `/database/${queryArg.databaseId}/runQuery`,
          method: 'POST',
          body: queryArg.runPostgresDatabaseQueryCommand,
        }),
        invalidatesTags: ['PostgresServerResource'],
      }),
      postApiResourcePostgresServerResourceByServerIdDatabase: build.mutation<
        PostApiResourcePostgresServerResourceByServerIdDatabaseApiResponse,
        PostApiResourcePostgresServerResourceByServerIdDatabaseApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource/${queryArg.serverId}/database`,
          method: 'POST',
          body: queryArg.createPostgresDatabaseCommand,
        }),
        invalidatesTags: ['PostgresServerResource'],
      }),
      postApiResourcePostgresServerResourceByServerIdContainerAction: build.mutation<
        PostApiResourcePostgresServerResourceByServerIdContainerActionApiResponse,
        PostApiResourcePostgresServerResourceByServerIdContainerActionApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/PostgresServerResource/${queryArg.serverId}/containerAction`,
          method: 'POST',
          params: {
            actionId: queryArg.actionId,
          },
        }),
        invalidatesTags: ['PostgresServerResource'],
      }),
      postApiResourceResourceGroup: build.mutation<
        PostApiResourceResourceGroupApiResponse,
        PostApiResourceResourceGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ResourceGroup`,
          method: 'POST',
          body: queryArg.createResourceGroupCommand,
        }),
        invalidatesTags: ['ResourceGroup'],
      }),
      getApiResourceResourceGroup: build.query<
        GetApiResourceResourceGroupApiResponse,
        GetApiResourceResourceGroupApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ResourceGroup`,
          params: {
            Page: queryArg.page,
            PageSize: queryArg.pageSize,
            FilterBy: queryArg.filterBy,
            OrderBy: queryArg.orderBy,
          },
        }),
        providesTags: ['ResourceGroup'],
      }),
      getApiResourceResourceGroupById: build.query<
        GetApiResourceResourceGroupByIdApiResponse,
        GetApiResourceResourceGroupByIdApiArg
      >({
        query: (queryArg) => ({ url: `/api/resource/ResourceGroup/${queryArg.id}` }),
        providesTags: ['ResourceGroup'],
      }),
      getApiResourceResourceGroupedResourceByResourceId: build.query<
        GetApiResourceResourceGroupedResourceByResourceIdApiResponse,
        GetApiResourceResourceGroupedResourceByResourceIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ResourceGroupedResource/${queryArg.resourceId}`,
        }),
        providesTags: ['ResourceGroupedResource'],
      }),
      postApiResourceReverseProxyConnectByResourceId: build.mutation<
        PostApiResourceReverseProxyConnectByResourceIdApiResponse,
        PostApiResourceReverseProxyConnectByResourceIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ReverseProxy/connect/${queryArg.resourceId}`,
          method: 'POST',
          body: queryArg.connectReverseProxyCommand,
        }),
        invalidatesTags: ['ReverseProxy'],
      }),
      postApiResourceReverseProxyDisconnectByResourceId: build.mutation<
        PostApiResourceReverseProxyDisconnectByResourceIdApiResponse,
        PostApiResourceReverseProxyDisconnectByResourceIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ReverseProxy/disconnect/${queryArg.resourceId}`,
          method: 'POST',
          body: queryArg.disconnectReverseProxyCommand,
        }),
        invalidatesTags: ['ReverseProxy'],
      }),
      getApiResourceReverseProxyPreCheckDns: build.query<
        GetApiResourceReverseProxyPreCheckDnsApiResponse,
        GetApiResourceReverseProxyPreCheckDnsApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/ReverseProxy/preCheckDns`,
          params: {
            url: queryArg.url,
            myAdminUrl: queryArg.myAdminUrl,
          },
        }),
        providesTags: ['ReverseProxy'],
      }),
      getHealth: build.query<GetHealthApiResponse, GetHealthApiArg>({
        query: () => ({ url: `/health` }),
        providesTags: ['Status'],
      }),
      getAuthenticated: build.query<GetAuthenticatedApiResponse, GetAuthenticatedApiArg>({
        query: () => ({ url: `/authenticated` }),
        providesTags: ['Status'],
      }),
      postApiTenantManagementCreateUser: build.mutation<
        PostApiTenantManagementCreateUserApiResponse,
        PostApiTenantManagementCreateUserApiArg
      >({
        query: (queryArg) => ({
          url: `/api/TenantManagement/createUser`,
          method: 'POST',
          body: queryArg.userRegistrationCommand,
        }),
        invalidatesTags: ['TenantManagement'],
      }),
      putApiTenantManagementUpdateUserById: build.mutation<
        PutApiTenantManagementUpdateUserByIdApiResponse,
        PutApiTenantManagementUpdateUserByIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/TenantManagement/updateUser/${queryArg.id}`,
          method: 'PUT',
          body: queryArg.userUpdateCommand,
        }),
        invalidatesTags: ['TenantManagement'],
      }),
      deleteApiTenantManagementDeleteUserById: build.mutation<
        DeleteApiTenantManagementDeleteUserByIdApiResponse,
        DeleteApiTenantManagementDeleteUserByIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/TenantManagement/deleteUser/${queryArg.id}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['TenantManagement'],
      }),
      getApiResourceVirtualNetworkResource: build.query<
        GetApiResourceVirtualNetworkResourceApiResponse,
        GetApiResourceVirtualNetworkResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/VirtualNetworkResource`,
          params: {
            Page: queryArg.page,
            PageSize: queryArg.pageSize,
            FilterBy: queryArg.filterBy,
            OrderBy: queryArg.orderBy,
          },
        }),
        providesTags: ['VirtualNetworkResource'],
      }),
      postApiResourceVirtualNetworkResource: build.mutation<
        PostApiResourceVirtualNetworkResourceApiResponse,
        PostApiResourceVirtualNetworkResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/VirtualNetworkResource`,
          method: 'POST',
          body: queryArg.createVirtualNetworkResourceCommand,
        }),
        invalidatesTags: ['VirtualNetworkResource'],
      }),
      getApiResourceVirtualNetworkResourceById: build.query<
        GetApiResourceVirtualNetworkResourceByIdApiResponse,
        GetApiResourceVirtualNetworkResourceByIdApiArg
      >({
        query: (queryArg) => ({ url: `/api/resource/VirtualNetworkResource/${queryArg.id}` }),
        providesTags: ['VirtualNetworkResource'],
      }),
      postApiResourceVirtualNetworkResourceByIdConnect: build.mutation<
        PostApiResourceVirtualNetworkResourceByIdConnectApiResponse,
        PostApiResourceVirtualNetworkResourceByIdConnectApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/VirtualNetworkResource/${queryArg.id}/connect`,
          method: 'POST',
          body: queryArg.connectVirtualNetworkCommand,
        }),
        invalidatesTags: ['VirtualNetworkResource'],
      }),
      postApiResourceWebApplicationResource: build.mutation<
        PostApiResourceWebApplicationResourceApiResponse,
        PostApiResourceWebApplicationResourceApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource`,
          method: 'POST',
          body: queryArg.createWebApplicationResourceCommand,
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
      getApiResourceWebApplicationResourceById: build.query<
        GetApiResourceWebApplicationResourceByIdApiResponse,
        GetApiResourceWebApplicationResourceByIdApiArg
      >({
        query: (queryArg) => ({ url: `/api/resource/WebApplicationResource/${queryArg.id}` }),
        providesTags: ['WebApplicationResource'],
      }),
      deleteApiResourceWebApplicationResourceById: build.mutation<
        DeleteApiResourceWebApplicationResourceByIdApiResponse,
        DeleteApiResourceWebApplicationResourceByIdApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource/${queryArg.id}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
      putApiResourceWebApplicationResourceByIdDeploymentConfiguration: build.mutation<
        PutApiResourceWebApplicationResourceByIdDeploymentConfigurationApiResponse,
        PutApiResourceWebApplicationResourceByIdDeploymentConfigurationApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource/${queryArg.id}/deploymentConfiguration`,
          method: 'PUT',
          body: queryArg.updateWebApplicationDeploymentConfigurationCommand,
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
      postApiResourceWebApplicationResourceByIdConfiguration: build.mutation<
        PostApiResourceWebApplicationResourceByIdConfigurationApiResponse,
        PostApiResourceWebApplicationResourceByIdConfigurationApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource/${queryArg.id}/configuration`,
          method: 'POST',
          body: queryArg.modifyWebApplicationConfigItemCommand,
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
      deleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKey: build.mutation<
        DeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyApiResponse,
        DeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource/${queryArg.id}/configuration/${queryArg.configurationKey}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
      postApiResourceWebApplicationResourceByServerIdContainerAction: build.mutation<
        PostApiResourceWebApplicationResourceByServerIdContainerActionApiResponse,
        PostApiResourceWebApplicationResourceByServerIdContainerActionApiArg
      >({
        query: (queryArg) => ({
          url: `/api/resource/WebApplicationResource/${queryArg.serverId}/containerAction`,
          method: 'POST',
          params: {
            actionId: queryArg.actionId,
          },
        }),
        invalidatesTags: ['WebApplicationResource'],
      }),
    }),
    overrideExisting: false,
  });
export { injectedRtkApi as cloudyApi };
export type PostApiAuthenticationLoginApiResponse = /** status 200 OK */ LoggedInUserDto;
export type PostApiAuthenticationLoginApiArg = {
  userLoginCommand: UserLoginCommand;
};
export type GetApiResourceBaseResourceApiResponse =
  /** status 200 OK */ BaseResourceDtoPaginatedResponse;
export type GetApiResourceBaseResourceApiArg = {
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
};
export type GetApiResourceBaseResourceDockerEnvironmentApiResponse =
  /** status 200 OK */ DockerEnvironment;
export type GetApiResourceBaseResourceDockerEnvironmentApiArg = void;
export type GetApiResourceBaseResourceByResourceIdContainerApiResponse =
  /** status 200 OK */ ContainerDto;
export type GetApiResourceBaseResourceByResourceIdContainerApiArg = {
  resourceId: string;
};
export type GetApiResourceBaseResourceByResourceIdAuditLogApiResponse =
  /** status 200 OK */ AuditLogEntryPaginatedResponse;
export type GetApiResourceBaseResourceByResourceIdAuditLogApiArg = {
  resourceId: string;
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
};
export type PostApiResourceBaseResourceByResourceIdPrometheusApiResponse =
  /** status 200 OK */ PrometheusResultDto;
export type PostApiResourceBaseResourceByResourceIdPrometheusApiArg = {
  resourceId: string;
  queryPrometheusQuery: QueryPrometheusQuery;
};
export type PostApiResourceBaseResourceByResourceIdLokiApiResponse =
  /** status 200 OK */ PrometheusResultDto;
export type PostApiResourceBaseResourceByResourceIdLokiApiArg = {
  resourceId: string;
  queryLokiQuery: QueryLokiQuery;
};
export type GetApiResourceKafkaClusterResourceByServerIdApiResponse =
  /** status 200 OK */ KafkaClusterResourceDto;
export type GetApiResourceKafkaClusterResourceByServerIdApiArg = {
  serverId: string;
};
export type DeleteApiResourceKafkaClusterResourceByServerIdApiResponse = unknown;
export type DeleteApiResourceKafkaClusterResourceByServerIdApiArg = {
  serverId: string;
};
export type GetApiResourceKafkaClusterResourceByServerIdTopicsApiResponse =
  /** status 200 OK */ KafkaTopicDto[];
export type GetApiResourceKafkaClusterResourceByServerIdTopicsApiArg = {
  serverId: string;
};
export type PostApiResourceKafkaClusterResourceByServerIdTopicsApiResponse = unknown;
export type PostApiResourceKafkaClusterResourceByServerIdTopicsApiArg = {
  serverId: string;
  createKafkaTopicCommand: CreateKafkaTopicCommand;
};
export type PostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdApiResponse = unknown;
export type PostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdApiArg = {
  serverId: string;
  topicId: string;
  produceKafkaTopicMessageCommand: ProduceKafkaTopicMessageCommand;
};
export type GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiResponse =
  /** status 200 OK */ string[];
export type GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiArg = {
  serverId: string;
  topicId: string;
};
export type PostApiResourceKafkaClusterResourceApiResponse =
  /** status 200 OK */ KafkaClusterResourceDto;
export type PostApiResourceKafkaClusterResourceApiArg = {
  createKafkaClusterCommand: CreateKafkaClusterCommand;
};
export type PostApiResourceKafkaClusterResourceByServerIdContainerActionApiResponse = unknown;
export type PostApiResourceKafkaClusterResourceByServerIdContainerActionApiArg = {
  serverId: string;
  actionId?: string;
};
export type GetApiResourcePostgresServerResourceApiResponse =
  /** status 200 OK */ PostgresServerResourceDtoPaginatedResponse;
export type GetApiResourcePostgresServerResourceApiArg = {
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
};
export type PostApiResourcePostgresServerResourceApiResponse =
  /** status 200 OK */ PostgresServerResourceDto;
export type PostApiResourcePostgresServerResourceApiArg = {
  createPostgresServerCommand: CreatePostgresServerCommand;
};
export type GetApiResourcePostgresServerResourceByServerIdApiResponse =
  /** status 200 OK */ PostgresServerResourceDto;
export type GetApiResourcePostgresServerResourceByServerIdApiArg = {
  serverId: string;
};
export type DeleteApiResourcePostgresServerResourceByServerIdApiResponse = unknown;
export type DeleteApiResourcePostgresServerResourceByServerIdApiArg = {
  serverId: string;
};
export type GetApiResourcePostgresServerResourceDatabaseByDatabaseIdApiResponse =
  /** status 200 OK */ PostgresDatabaseResourceDto;
export type GetApiResourcePostgresServerResourceDatabaseByDatabaseIdApiArg = {
  databaseId: string;
};
export type PostDatabaseByDatabaseIdRunQueryApiResponse =
  /** status 200 OK */ PostgresQueryResultDto;
export type PostDatabaseByDatabaseIdRunQueryApiArg = {
  databaseId: string;
  runPostgresDatabaseQueryCommand: RunPostgresDatabaseQueryCommand;
};
export type PostApiResourcePostgresServerResourceByServerIdDatabaseApiResponse =
  /** status 200 OK */ PostgresDatabaseSimpleResourceDto;
export type PostApiResourcePostgresServerResourceByServerIdDatabaseApiArg = {
  serverId: string;
  createPostgresDatabaseCommand: CreatePostgresDatabaseCommand;
};
export type PostApiResourcePostgresServerResourceByServerIdContainerActionApiResponse = unknown;
export type PostApiResourcePostgresServerResourceByServerIdContainerActionApiArg = {
  serverId: string;
  actionId?: string;
};
export type PostApiResourceResourceGroupApiResponse = /** status 200 OK */ ResourceGroupSimpleDto;
export type PostApiResourceResourceGroupApiArg = {
  createResourceGroupCommand: CreateResourceGroupCommand;
};
export type GetApiResourceResourceGroupApiResponse =
  /** status 200 OK */ ResourceGroupSimpleDtoPaginatedResponse;
export type GetApiResourceResourceGroupApiArg = {
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
};
export type GetApiResourceResourceGroupByIdApiResponse = /** status 200 OK */ ResourceGroupDto;
export type GetApiResourceResourceGroupByIdApiArg = {
  id: string;
};
export type GetApiResourceResourceGroupedResourceByResourceIdApiResponse =
  /** status 200 OK */ ResourceGroupedBaseResourceDto;
export type GetApiResourceResourceGroupedResourceByResourceIdApiArg = {
  resourceId: string;
};
export type PostApiResourceReverseProxyConnectByResourceIdApiResponse = unknown;
export type PostApiResourceReverseProxyConnectByResourceIdApiArg = {
  resourceId: string;
  connectReverseProxyCommand: ConnectReverseProxyCommand;
};
export type PostApiResourceReverseProxyDisconnectByResourceIdApiResponse = unknown;
export type PostApiResourceReverseProxyDisconnectByResourceIdApiArg = {
  resourceId: string;
  disconnectReverseProxyCommand: DisconnectReverseProxyCommand;
};
export type GetApiResourceReverseProxyPreCheckDnsApiResponse = /** status 200 OK */ DnsCheckDto;
export type GetApiResourceReverseProxyPreCheckDnsApiArg = {
  url?: string;
  myAdminUrl?: string;
};
export type GetHealthApiResponse = /** status 200 OK */ string;
export type GetHealthApiArg = void;
export type GetAuthenticatedApiResponse = /** status 200 OK */ string;
export type GetAuthenticatedApiArg = void;
export type PostApiTenantManagementCreateUserApiResponse = unknown;
export type PostApiTenantManagementCreateUserApiArg = {
  userRegistrationCommand: UserRegistrationCommand;
};
export type PutApiTenantManagementUpdateUserByIdApiResponse = unknown;
export type PutApiTenantManagementUpdateUserByIdApiArg = {
  id: string;
  userUpdateCommand: UserUpdateCommand;
};
export type DeleteApiTenantManagementDeleteUserByIdApiResponse = unknown;
export type DeleteApiTenantManagementDeleteUserByIdApiArg = {
  id: string;
};
export type GetApiResourceVirtualNetworkResourceApiResponse =
  /** status 200 OK */ VirtualNetworkSimpleDtoPaginatedResponse;
export type GetApiResourceVirtualNetworkResourceApiArg = {
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
};
export type PostApiResourceVirtualNetworkResourceApiResponse =
  /** status 200 OK */ VirtualNetworkSimpleDto;
export type PostApiResourceVirtualNetworkResourceApiArg = {
  createVirtualNetworkResourceCommand: CreateVirtualNetworkResourceCommand;
};
export type GetApiResourceVirtualNetworkResourceByIdApiResponse =
  /** status 200 OK */ VirtualNetworkResourceDto;
export type GetApiResourceVirtualNetworkResourceByIdApiArg = {
  id: string;
};
export type PostApiResourceVirtualNetworkResourceByIdConnectApiResponse = unknown;
export type PostApiResourceVirtualNetworkResourceByIdConnectApiArg = {
  id: string;
  connectVirtualNetworkCommand: ConnectVirtualNetworkCommand;
};
export type PostApiResourceWebApplicationResourceApiResponse =
  /** status 200 OK */ WebApplicationResourceDto;
export type PostApiResourceWebApplicationResourceApiArg = {
  createWebApplicationResourceCommand: CreateWebApplicationResourceCommand;
};
export type GetApiResourceWebApplicationResourceByIdApiResponse =
  /** status 200 OK */ WebApplicationResourceDto;
export type GetApiResourceWebApplicationResourceByIdApiArg = {
  id: string;
};
export type DeleteApiResourceWebApplicationResourceByIdApiResponse = unknown;
export type DeleteApiResourceWebApplicationResourceByIdApiArg = {
  id: string;
};
export type PutApiResourceWebApplicationResourceByIdDeploymentConfigurationApiResponse = unknown;
export type PutApiResourceWebApplicationResourceByIdDeploymentConfigurationApiArg = {
  id: string;
  updateWebApplicationDeploymentConfigurationCommand: UpdateWebApplicationDeploymentConfigurationCommand;
};
export type PostApiResourceWebApplicationResourceByIdConfigurationApiResponse = unknown;
export type PostApiResourceWebApplicationResourceByIdConfigurationApiArg = {
  id: string;
  modifyWebApplicationConfigItemCommand: ModifyWebApplicationConfigItemCommand;
};
export type DeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyApiResponse =
  unknown;
export type DeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyApiArg = {
  id: string;
  configurationKey: string;
};
export type PostApiResourceWebApplicationResourceByServerIdContainerActionApiResponse = unknown;
export type PostApiResourceWebApplicationResourceByServerIdContainerActionApiArg = {
  serverId: string;
  actionId?: string;
};
export type LoggedInUserDto = {
  id?: string;
  email?: string;
  token?: string;
};
export type ProblemDetails = {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: any;
};
export type UserLoginCommand = {
  email?: string;
  password?: string;
};
export type UserLoginCommandRead = {
  email?: string;
  normalizedEmail?: string;
  password?: string;
};
export type BaseResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
};
export type BaseResourceDtoPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: BaseResourceDto[];
};
export type DockerContainer = {
  containerId?: string;
  containerName?: string;
  stateRunning?: boolean;
  statePaused?: boolean;
  stateRestarting?: boolean;
  stateError?: string;
  stateStartedAt?: string | null;
  stateFinishedAt?: string | null;
  networkIds?: string[] | null;
  volumeIds?: string[] | null;
};
export type DockerImage = {
  imageId?: string;
  tag?: string;
  createdAt?: string;
  size?: number;
};
export type DockerNetwork = {
  networkId?: string;
  name?: string;
  containerIds?: string[];
};
export type DockerVolume = {
  volumeId?: string;
  name?: string;
  size?: number;
  createdAt?: string;
};
export type DockerEnvironment = {
  containers?: DockerContainer[];
  images?: DockerImage[];
  networks?: DockerNetwork[];
  volumes?: DockerVolume[];
};
export type ContainerDto = {
  stateRunning?: boolean;
  statePaused?: boolean;
  stateRestarting?: boolean;
  stateError?: string;
  stateStartedAt?: string;
  stateFinishedAt?: string;
};
export type BaseResource = {
  id?: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  auditLogEntries?: AuditLogEntry[];
};
export type AuditLogEntry = {
  id?: string;
  actionName: string;
  actionDisplayText: string;
  actionMetadata?: string | null;
  timestamp?: string;
  resourceId?: string;
  resource?: BaseResource;
};
export type AuditLogEntryPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: AuditLogEntry[];
};
export type PrometheusResultItemDto = {
  metric?: {
    [key: string]: string;
  };
  stream?: {
    [key: string]: string;
  };
  values?: any[][];
};
export type PrometheusDataDto = {
  resultType?: string;
  result?: PrometheusResultItemDto[];
};
export type PrometheusResultDto = {
  status?: string;
  data?: PrometheusDataDto;
};
export type QueryPrometheusQuery = {
  query?: PredefinedPrometheusQuery;
  start?: string;
  end?: string;
  step?: string | null;
  timeout?: string | null;
  limit?: number | null;
};
export type QueryLokiQuery = {
  query?: PredefinedLokiQuery;
  start?: string;
  end?: string;
  step?: string | null;
  timeout?: string | null;
  limit?: number | null;
};
export type VirtualNetworkSimpleDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
};
export type PublicProxyConfigurationDto = {
  id?: string;
  useHttps?: boolean;
  domain?: string;
  port?: number;
};
export type KafkaClusterResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
  virtualNetworks?: VirtualNetworkSimpleDto[];
  publicProxyConfigurations?: PublicProxyConfigurationDto[];
};
export type KafkaPartitionDto = {
  topic?: string;
  partition?: number;
  leader?: number;
  replicas?: number[];
  isr?: number[];
  elr?: string;
  lastKnownElr?: string;
};
export type KafkaTopicDto = {
  name?: string;
  topicId?: string;
  partitionCount?: number;
  replicationFactor?: number;
  partitions?: KafkaPartitionDto[];
};
export type CreateKafkaTopicCommand = {
  topicName?: string;
};
export type ProduceKafkaTopicMessageCommand = {
  key?: string;
  value?: string;
};
export type CreateKafkaClusterCommand = {
  resourceGroupId: string;
  serverName: string;
  serverPort?: number | null;
};
export type PostgresDatabaseSimpleResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  databaseName?: string;
  adminUsername?: string;
};
export type PostgresServerResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
  virtualNetworks?: VirtualNetworkSimpleDto[];
  publicProxyConfigurations?: PublicProxyConfigurationDto[];
  postgresDatabaseResources?: PostgresDatabaseSimpleResourceDto[];
};
export type PostgresServerResourceDtoPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: PostgresServerResourceDto[];
};
export type CreatePostgresServerCommand = {
  resourceGroupId: string;
  serverName: string;
  serverPort?: number | null;
};
export type PostgresDatabaseResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
  databaseName?: string;
  adminUsername?: string;
  serverName?: string;
  serverId?: string;
};
export type PostgresQueryResultDto = {
  csvResponse?: string[][];
};
export type RunPostgresDatabaseQueryCommand = {
  query?: string;
};
export type CreatePostgresDatabaseCommand = {
  databaseName: string;
  adminUsername: string;
  adminPassword: string;
};
export type ResourceGroupSimpleDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
};
export type CreateResourceGroupCommand = {
  name: string;
};
export type ResourceGroupSimpleDtoPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: ResourceGroupSimpleDto[];
};
export type ResourceGroupDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resources?: BaseResourceDto[];
};
export type ResourceGroupedBaseResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
};
export type ConnectReverseProxyCommand = {
  urlForHost?: string;
  useHttps?: boolean;
};
export type DisconnectReverseProxyCommand = {
  connectionId?: string;
};
export type DnsCheckDto = {
  hostname?: string;
  ipsBehindHostname?: string[];
  adminHostname?: string;
  ipsBehindAdminHostname?: string[];
};
export type UserRegistrationCommand = {
  email?: string;
  password?: string;
};
export type UserRegistrationCommandRead = {
  email?: string;
  normalizedEmail?: string;
  password?: string;
};
export type UserUpdateCommand = {
  newPassword?: string | null;
};
export type VirtualNetworkSimpleDtoPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: VirtualNetworkSimpleDto[];
};
export type CreateVirtualNetworkResourceCommand = {
  resourceGroupId?: string;
  name?: string;
};
export type VirtualNetworkResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
  resources?: BaseResourceDto[];
};
export type ConnectVirtualNetworkCommand = {
  networkId?: string;
  resourceId?: string;
};
export type WebApplicationConfigurationEntryDto = {
  key?: string;
  value?: string;
};
export type WebApplicationResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
  virtualNetworks?: VirtualNetworkSimpleDto[];
  publicProxyConfigurations?: PublicProxyConfigurationDto[];
  sourcePath?: string;
  sourceType?: WebApplicationSourceType;
  buildCommand?: string;
  startupCommand?: string;
  runtimeType?: WebApplicationRuntimeType;
  healthCheckUrl?: string;
  port?: number;
  configuration?: WebApplicationConfigurationEntryDto[] | null;
};
export type CreateWebApplicationResourceCommand = {
  webApplicationName?: string;
  resourceGroupId?: string;
  sourcePath?: string;
  sourceType?: WebApplicationSourceType;
};
export type UpdateWebApplicationDeploymentConfigurationCommand = {
  buildCommand?: string;
  startupCommand?: string;
  runtimeType?: WebApplicationRuntimeType;
  port?: number;
};
export type ModifyWebApplicationConfigItemCommand = {
  key?: string;
  value?: string;
};
export enum PredefinedPrometheusQuery {
  HttpRequestsCount = 'HttpRequestsCount',
  PostgresProcessesCount = 'PostgresProcessesCount',
  PostgresEntriesInserted = 'PostgresEntriesInserted',
  PostgresEntriesReturned = 'PostgresEntriesReturned',
  GeneralCpuLoad = 'GeneralCPULoad',
  GeneralMemoryUsage = 'GeneralMemoryUsage',
}
export enum PredefinedLokiQuery {
  ContainerLog = 'ContainerLog',
}
export enum WebApplicationSourceType {
  PublicGitClone = 'PublicGitClone',
}
export enum WebApplicationRuntimeType {
  Python = 'Python',
  NodeJs = 'NodeJs',
  DotNet = 'DotNet',
}
export const {
  usePostApiAuthenticationLoginMutation,
  useGetApiResourceBaseResourceQuery,
  useLazyGetApiResourceBaseResourceQuery,
  useGetApiResourceBaseResourceDockerEnvironmentQuery,
  useLazyGetApiResourceBaseResourceDockerEnvironmentQuery,
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useLazyGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourceBaseResourceByResourceIdAuditLogQuery,
  useLazyGetApiResourceBaseResourceByResourceIdAuditLogQuery,
  usePostApiResourceBaseResourceByResourceIdPrometheusMutation,
  usePostApiResourceBaseResourceByResourceIdLokiMutation,
  useGetApiResourceKafkaClusterResourceByServerIdQuery,
  useLazyGetApiResourceKafkaClusterResourceByServerIdQuery,
  useDeleteApiResourceKafkaClusterResourceByServerIdMutation,
  useGetApiResourceKafkaClusterResourceByServerIdTopicsQuery,
  useLazyGetApiResourceKafkaClusterResourceByServerIdTopicsQuery,
  usePostApiResourceKafkaClusterResourceByServerIdTopicsMutation,
  usePostApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdMutation,
  useGetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery,
  useLazyGetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery,
  usePostApiResourceKafkaClusterResourceMutation,
  usePostApiResourceKafkaClusterResourceByServerIdContainerActionMutation,
  useGetApiResourcePostgresServerResourceQuery,
  useLazyGetApiResourcePostgresServerResourceQuery,
  usePostApiResourcePostgresServerResourceMutation,
  useGetApiResourcePostgresServerResourceByServerIdQuery,
  useLazyGetApiResourcePostgresServerResourceByServerIdQuery,
  useDeleteApiResourcePostgresServerResourceByServerIdMutation,
  useGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery,
  useLazyGetApiResourcePostgresServerResourceDatabaseByDatabaseIdQuery,
  usePostDatabaseByDatabaseIdRunQueryMutation,
  usePostApiResourcePostgresServerResourceByServerIdDatabaseMutation,
  usePostApiResourcePostgresServerResourceByServerIdContainerActionMutation,
  usePostApiResourceResourceGroupMutation,
  useGetApiResourceResourceGroupQuery,
  useLazyGetApiResourceResourceGroupQuery,
  useGetApiResourceResourceGroupByIdQuery,
  useLazyGetApiResourceResourceGroupByIdQuery,
  useGetApiResourceResourceGroupedResourceByResourceIdQuery,
  useLazyGetApiResourceResourceGroupedResourceByResourceIdQuery,
  usePostApiResourceReverseProxyConnectByResourceIdMutation,
  usePostApiResourceReverseProxyDisconnectByResourceIdMutation,
  useGetApiResourceReverseProxyPreCheckDnsQuery,
  useLazyGetApiResourceReverseProxyPreCheckDnsQuery,
  useGetHealthQuery,
  useLazyGetHealthQuery,
  useGetAuthenticatedQuery,
  useLazyGetAuthenticatedQuery,
  usePostApiTenantManagementCreateUserMutation,
  usePutApiTenantManagementUpdateUserByIdMutation,
  useDeleteApiTenantManagementDeleteUserByIdMutation,
  useGetApiResourceVirtualNetworkResourceQuery,
  useLazyGetApiResourceVirtualNetworkResourceQuery,
  usePostApiResourceVirtualNetworkResourceMutation,
  useGetApiResourceVirtualNetworkResourceByIdQuery,
  useLazyGetApiResourceVirtualNetworkResourceByIdQuery,
  usePostApiResourceVirtualNetworkResourceByIdConnectMutation,
  usePostApiResourceWebApplicationResourceMutation,
  useGetApiResourceWebApplicationResourceByIdQuery,
  useLazyGetApiResourceWebApplicationResourceByIdQuery,
  useDeleteApiResourceWebApplicationResourceByIdMutation,
  usePutApiResourceWebApplicationResourceByIdDeploymentConfigurationMutation,
  usePostApiResourceWebApplicationResourceByIdConfigurationMutation,
  useDeleteApiResourceWebApplicationResourceByIdConfigurationAndConfigurationKeyMutation,
  usePostApiResourceWebApplicationResourceByServerIdContainerActionMutation,
} = injectedRtkApi;
