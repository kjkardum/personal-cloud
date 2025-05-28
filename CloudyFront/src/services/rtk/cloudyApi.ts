import { api } from './api';

export const addTagTypes = [
  'Authentication',
  'BaseResource',
  'KafkaClusterResource',
  'PostgresServerResource',
  'ResourceGroup',
  'ResourceGroupedResource',
  'Status',
  'TenantManagement',
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
export type PostApiResourceKafkaClusterResourceApiResponse =
  /** status 200 OK */ KafkaClusterResourceDto;
export type PostApiResourceKafkaClusterResourceApiArg = {
  createKafkaClusterCommand: CreateKafkaClusterCommand;
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
export type KafkaClusterResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
  resourceGroupId?: string;
  resourceGroupName?: string;
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
export enum PredefinedPrometheusQuery {
  PostgresProcessesCount = 'PostgresProcessesCount',
  PostgresEntriesInserted = 'PostgresEntriesInserted',
  PostgresEntriesReturned = 'PostgresEntriesReturned',
  GeneralCpuLoad = 'GeneralCPULoad',
  GeneralMemoryUsage = 'GeneralMemoryUsage',
}
export enum PredefinedLokiQuery {
  Demo = 'Demo',
}
export const {
  usePostApiAuthenticationLoginMutation,
  useGetApiResourceBaseResourceQuery,
  useLazyGetApiResourceBaseResourceQuery,
  useGetApiResourceBaseResourceByResourceIdContainerQuery,
  useLazyGetApiResourceBaseResourceByResourceIdContainerQuery,
  useGetApiResourceBaseResourceByResourceIdAuditLogQuery,
  useLazyGetApiResourceBaseResourceByResourceIdAuditLogQuery,
  usePostApiResourceBaseResourceByResourceIdPrometheusMutation,
  usePostApiResourceBaseResourceByResourceIdLokiMutation,
  useGetApiResourceKafkaClusterResourceByServerIdQuery,
  useLazyGetApiResourceKafkaClusterResourceByServerIdQuery,
  usePostApiResourceKafkaClusterResourceMutation,
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
  useGetHealthQuery,
  useLazyGetHealthQuery,
  useGetAuthenticatedQuery,
  useLazyGetAuthenticatedQuery,
  usePostApiTenantManagementCreateUserMutation,
  usePutApiTenantManagementUpdateUserByIdMutation,
  useDeleteApiTenantManagementDeleteUserByIdMutation,
} = injectedRtkApi;
