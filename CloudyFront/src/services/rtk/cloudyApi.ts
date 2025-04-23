import { api } from './api';

export const addTagTypes = [
  'Authentication',
  'BaseResource',
  'PostgresServerResource',
  'ResourceGroup',
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
export type PostApiResourcePostgresServerResourceByServerIdDatabaseApiResponse =
  /** status 200 OK */ PostgresDatabaseResourceDto;
export type PostApiResourcePostgresServerResourceByServerIdDatabaseApiArg = {
  serverId: string;
  createPostgresDatabaseCommand: CreatePostgresDatabaseCommand;
};
export type PostApiResourceResourceGroupApiResponse = /** status 200 OK */ ResourceGroupDto;
export type PostApiResourceResourceGroupApiArg = {
  createResourceGroupCommand: CreateResourceGroupCommand;
};
export type GetApiResourceResourceGroupApiResponse =
  /** status 200 OK */ ResourceGroupDtoPaginatedResponse;
export type GetApiResourceResourceGroupApiArg = {
  page?: number;
  pageSize?: number;
  filterBy?: string;
  orderBy?: string;
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
export type PostgresServerResourceDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
  resourceType: string;
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
};
export type CreatePostgresDatabaseCommand = {
  databaseName: string;
  adminUsername: string;
  adminPassword: string;
};
export type ResourceGroupDto = {
  id: string;
  name: string;
  createdAt?: string;
  updatedAt?: string;
};
export type CreateResourceGroupCommand = {
  name: string;
};
export type ResourceGroupDtoPaginatedResponse = {
  page?: number;
  pageSize?: number;
  totalCount?: number;
  data: ResourceGroupDto[];
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
export const {
  usePostApiAuthenticationLoginMutation,
  useGetApiResourceBaseResourceQuery,
  useLazyGetApiResourceBaseResourceQuery,
  useGetApiResourcePostgresServerResourceQuery,
  useLazyGetApiResourcePostgresServerResourceQuery,
  usePostApiResourcePostgresServerResourceMutation,
  useGetApiResourcePostgresServerResourceByServerIdQuery,
  useLazyGetApiResourcePostgresServerResourceByServerIdQuery,
  usePostApiResourcePostgresServerResourceByServerIdDatabaseMutation,
  usePostApiResourceResourceGroupMutation,
  useGetApiResourceResourceGroupQuery,
  useLazyGetApiResourceResourceGroupQuery,
  useGetHealthQuery,
  useLazyGetHealthQuery,
  useGetAuthenticatedQuery,
  useLazyGetAuthenticatedQuery,
  usePostApiTenantManagementCreateUserMutation,
  usePutApiTenantManagementUpdateUserByIdMutation,
  useDeleteApiTenantManagementDeleteUserByIdMutation,
} = injectedRtkApi;
