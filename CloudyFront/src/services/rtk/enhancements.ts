import {
  cloudyApi,
  GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiArg,
} from '@/services/rtk/cloudyApi';
import { API_BASE_URL } from '@/config';
import { ApiHelperState, getAbortControllerForStream } from '@/store/stores/ApiHelperStore';

const apiWithMoreEndpoints = cloudyApi.injectEndpoints({
  endpoints: (build) => ({
    streamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLive: build.query<
      string[],
      GetApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveApiArg
    >({
      queryFn: async () => ({ data: [] }),
      providesTags: ['KafkaClusterResource'],
      async onCacheEntryAdded(
        queryArg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved, getState }
      ) {
        try {
          await cacheDataLoaded;

          // @ts-expect-error;
          const apiHelper = getState().apiHelper as ApiHelperState
          const abortController = getAbortControllerForStream(apiHelper, queryArg.serverId, queryArg.topicId);
          if (!abortController) {
            throw new Error(`No active stream found for ${queryArg.serverId}/${queryArg.topicId}`);
          }

          const response = await fetch(
            `${API_BASE_URL}/api/resource/KafkaClusterResource/${queryArg.serverId}/topics/${queryArg.topicId}/consumeLive`,
            {
              method: 'GET',
              headers: {
                'Accept': 'text/plain',
                'Cache-Control': 'no-cache',
                'Connection': 'keep-alive'
              },
              credentials: 'include',
              signal: abortController.signal,
            }
          );

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
          }

          if (!response.body) {
            throw new Error('ReadableStream not supported');
          }

          console.log('Gettting reader');

          // Use standard TextDecoder instead of TextDecoderStream
          const reader = response.body.getReader();
          const decoder = new TextDecoder();
          let buffer = '';
          while (true) {
            const { done, value } = await reader.read();

            if (done) break;

            // Decode the Uint8Array to string
            const chunk = decoder.decode(value, { stream: true });
            buffer += chunk;
            const currentResult = JSON.parse(`${buffer  }]`);
            console.log('Current result:', currentResult); // Debug log
            updateCachedData(draft => {
              draft.length = 0;
              draft.push(...currentResult);
            });
          }
        } catch (error) {
          console.error('Streaming error:', error);
        }

        await cacheEntryRemoved;
      },
    }),
  })
});

export const {
  useStreamApiResourceKafkaClusterResourceByServerIdTopicsAndTopicIdConsumeLiveQuery,
} = apiWithMoreEndpoints;
