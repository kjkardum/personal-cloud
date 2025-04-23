import { createSlice } from "@reduxjs/toolkit";
import { revertAll } from "@/store/actions/revertAll";

const initialState: UserState = {};

const slice = createSlice({
    name: "user",
    initialState,
    reducers: {
        loginUser: (state, { payload: { email, token, userId, refreshToken } }: UserPayload) => {
            state.email = email;
            state.token = token;
            state.refreshToken = refreshToken;
            state.userId = userId;
            state.isAuthenticated = true;
        },
        logoutUser: (state) => {
            state.email = undefined;
            state.token = undefined;
            state.refreshToken = undefined;
            state.userId = undefined;
            state.isAuthenticated = false;
        },
    },
    extraReducers: (builder) => {
        builder.addCase(revertAll, () => initialState);
    },
});

export const { loginUser, logoutUser } = slice.actions;

export default slice.reducer;

export type UserState = {
    email?: string;
    token?: string;
    refreshToken?: string;
    userId?: string;
    isInitialized?: boolean;
    isAuthenticated?: boolean;
};

type UserPayload = {
    payload: Partial<UserState>;
};
