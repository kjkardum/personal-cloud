import { createSlice } from '@reduxjs/toolkit';
import { revertAll } from '@/store/actions/revertAll';


const initialState: ApiHelperState = { activeStreams: [] };

const slice = createSlice({
  name: "apiHelper",
  initialState,
  reducers: {
    startTopicStream: (state, { payload: { serverId, topic}}: { payload: { serverId: string, topic: string } }) => {
      console.log(`startTopicStream: ${serverId}/${topic}`);
      if (!state.activeStreams.find((stream) => stream.id === `${serverId}/${topic}`)) {
        console.log(`Creating new stream for ${serverId}/${topic}`);
        state.activeStreams.push({id: `${serverId}/${topic}`, controller: new AbortController()});
      }
    },
    stopTopicStream: (state, { payload: { serverId, topic }}: { payload: { serverId: string, topic: string } }) => {
      console.log(`stopTopicStream: ${serverId}/${topic}`);
      const stream = state.activeStreams.find((stream) => stream.id === `${serverId}/${topic}`);
      if (!stream) {
        console.warn(`No active stream found for ${serverId}/${topic}`);
        return;
      }
      if (stream && stream.controller) {
        console.log(`Stopping stream for ${serverId}/${topic}`);
        stream.controller.abort();
        state.activeStreams = state.activeStreams.filter((s) => s.id !== stream.id);
      }
    },
  },
  extraReducers: (builder) => {
    builder.addCase(revertAll, () => initialState);
  },
});

export const getAbortControllerForStream = (state: ApiHelperState, serverId: string, topic: string) => {
  console.log(`getAbortControllerForStream: ${serverId}/${topic}`);
  const stream = state.activeStreams.find((stream) => stream.id === `${serverId}/${topic}`);
  if (!stream) {
    console.warn(`No active stream found for ${serverId}/${topic}`);
    return null;
  }
  return stream?.controller;
}

export const { startTopicStream, stopTopicStream } = slice.actions;

export default slice.reducer;

export type ApiHelperState = {
  activeStreams: {id: string, controller: AbortController}[];
};
