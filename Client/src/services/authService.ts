import apiClient from "./apiClient.ts";
import {LoginModel, LoginResponse} from "@/types";
import {AxiosResponse} from "axios";

const login = async (request: LoginModel): Promise<AxiosResponse<LoginResponse>> => {
     return await apiClient.post<LoginResponse>('/api/auth/login', request, {
        withCredentials: true
    });
}
const logout = async () => {
    try {
        var response = await apiClient.post('/api/auth/logout', null, {
            withCredentials: true
        });

        return response.status;
    } catch (error: any) {
        console.error('Error during logout:', error);
        if (error.response) {
            return error.response.status;
        } else {
            return 500;
        }

    }
}

export const authService = {
    login,
    logout
};