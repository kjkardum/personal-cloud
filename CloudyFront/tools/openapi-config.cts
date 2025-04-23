import type { ConfigFile } from "@rtk-query/codegen-openapi";

const config: ConfigFile = {
    schemaFile: "../apispecs/swagger.yaml",
    apiFile: "../src/services/rtk/api.ts",
    apiImport: "api",
    outputFile: "../src/services/rtk/cloudyApi.ts",
    exportName: "cloudyApi",
    useEnumType: true,
    tag: true,
    hooks: { queries: true, lazyQueries: true, mutations: true },
    endpointOverrides: [
        {
            pattern: /GetPaginated/,
            type: "query",
        },
    ],
};

export default config;
