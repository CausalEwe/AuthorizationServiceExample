import apiClient from "./apiClient.ts";
import {LoginModel, LoginResponse} from "@/types";
import {AxiosResponse} from "axios";

 const login = async (request: LoginModel): Promise<AxiosResponse<LoginResponse>> => {
     return await apiClient.post<LoginResponse>('/api/auth/login', request, {
        withCredentials: true
    });
}

export const authService = {
    login,
};