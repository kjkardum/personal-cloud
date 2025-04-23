import storage from "redux-persist/lib/storage";

export const persistConfig = {
    key: "root",
    storage: storage,
    blacklist: ["api", "calendar", "billing", "licenses", "dataTable"],
};
