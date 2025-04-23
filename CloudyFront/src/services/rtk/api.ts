import { BaseQueryFn, createApi, fetchBaseQuery, FetchArgs, FetchBaseQueryError } from "@reduxjs/toolkit/query/react";
import {API_BASE_URL} from "@/config";


export const baseQueryWithInterceptor: BaseQueryFn<string | FetchArgs, any, FetchBaseQueryError> = async (args, api, extraOptions) => {
    //nProgress.start();

    try {
        const baseQuery = fetchBaseQuery({
            baseUrl: API_BASE_URL,
            credentials: "include",
        });


        // eslint-disable-next-line prefer-const
        let result = await baseQuery(args, api, extraOptions);
/*
        THIS IS FOR ONE DAY TO DO REFRESH TOKENS
        if (
            result.error &&
            result.error.status === 401 &&
            (typeof args !== "object" || (args.url !== "/api/Authentication/RefreshToken" && args.url !== "/api/Authentication/Login"))
        ) {
            await api.dispatch(cloudyApi.endpoints.postApiAccountRefreshToken.initiate()).unwrap();

            result = await baseQuery(args, api, extraOptions);

            //nProgress.done();
        }
 */

        return result;
    } catch (error: any) {
            return {
                error,
            };
    } finally {
        //nProgress.done();
    }
};

export const api = createApi({
    baseQuery: baseQueryWithInterceptor,
    endpoints: () => ({}),
});

export type ApiType = typeof api;
