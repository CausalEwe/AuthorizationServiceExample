import {defineStore} from 'pinia';
import {ref} from 'vue';
import {UpdateUserModel, ShortUserModel} from "@/types";
import {userService} from "@/services/userService.ts";

export const useUsersStore = defineStore('users', () => {
    const total = ref<number>(0);
    const users = ref<Array<ShortUserModel>>([]);
    const modifiedUsers = ref<Array<UpdateUserModel>>([]);

    const fetchUsers = async () => {
        try {
            const response = await userService.getUsers();
            total.value = response.data.total;
            users.value = response.data.items;
        } catch (error) {
            console.error('Failed to fetch users:', error);
        }
    };

    const modifyUser = (userId: number, isActive: boolean) => {
        const existingIndex = modifiedUsers.value.findIndex((user) => user.userId === userId);
        if (existingIndex !== -1) {
            modifiedUsers.value[existingIndex].userId = userId;
            modifiedUsers.value[existingIndex].isActive = isActive;
        } else {
            modifiedUsers.value.push({userId, isActive});
        }
    };
    
    const saveChanges = async () => {
        try {
            for (const user of modifiedUsers.value) {
                await userService.updateUser({
                    userId: user.userId,
                    isActive: user.isActive
                });
            }
            await fetchUsers();
            modifiedUsers.value = [];
        } catch (error) {
            console.error('Failed to save changes:', error);
        }
    };

    return {
        total,
        users,
        modifiedUsers,
        fetchUsers,
        modifyUser,
        saveChanges,
    };
});
