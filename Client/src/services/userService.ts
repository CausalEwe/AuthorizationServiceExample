import apiClient from '@/services/apiClient';
import {UpdateUserModel, ShortUserModel, PagedResult} from '@types';
import {AxiosResponse} from "axios";

const getUsers = async (): Promise<AxiosResponse<Promise<PagedResult<ShortUserModel>>>>  => await apiClient.get<PagedResult<ShortUserModel>>('/api/user/all');
const updateUser = async (command: UpdateUserModel) : Promise<AxiosResponse<true>>  => await apiClient.post(`/api/user/update`, command);
const meUser = async (): Promise<AxiosResponse<string>> => await apiClient.get('/api/users/me');

export const userService = {
    getUsers,
    updateUser,
    meUser
};
