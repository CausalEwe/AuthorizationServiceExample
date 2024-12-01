export type ShortUserModel = {
    id: number;
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

