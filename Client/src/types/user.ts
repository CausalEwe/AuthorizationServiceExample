export type ShortUserModel = {
    id: string;
    login: string;
    isActive: boolean;
};

export type ListUsersQuery = {
    skipCount: number;
    maxResultCount: number;
}

export type UpdateUserModel = {
    userId: number;
    isActive: boolean;
};

