import { combineReducers, configureStore, createListenerMiddleware } from "@reduxjs/toolkit";
import { setupListeners } from "@reduxjs/toolkit/query";
import { TypedUseSelectorHook, useSelector } from "react-redux";
import { FLUSH, PAUSE, PERSIST, persistReducer, persistStore, PURGE, REGISTER, REHYDRATE } from "redux-persist";
import { persistConfig } from "@/store/persistConfig";
import userStore from "@/store/stores/UserStore";
import {api} from "@/services/rtk/api";
import apiHelperStore from '@/store/stores/ApiHelperStore';

const reducers = combineReducers({
    api: api.reducer,
    apiHelper: apiHelperStore,
    user: userStore,
});

const persistedReducer = persistReducer(persistConfig, reducers);

const listenerMiddleware = createListenerMiddleware();

const store = configureStore({
    reducer: persistedReducer,
    middleware: (getDefaultMiddleware) => {
        const middlewares = getDefaultMiddleware({
            serializableCheck: {
                ignoredPaths: ['apiHelper.activeStreams'],
                ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
            },
        })
            .concat(api.middleware)
            .concat(listenerMiddleware.middleware);

        return middlewares;
    },
});

const persistor = persistStore(store);

setupListeners(store.dispatch);

type AppState = ReturnType<typeof store.getState>;
const useAppSelector: TypedUseSelectorHook<AppState> = useSelector;

export { persistor, store, useAppSelector };
export type { AppState };
