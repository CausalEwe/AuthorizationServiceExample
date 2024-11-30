export type PagedResult<T> = {
    total: number;
    items: T[];
};