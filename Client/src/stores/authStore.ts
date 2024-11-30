import {defineStore} from 'pinia';
import {ref} from 'vue';
import {authService} from '@/services/authService';
import {LoginModel} from "@types";

export const useAuthStore = defineStore('auth', () => {
    const token = ref<string | null>(localStorage.getItem('token'));
    const errorMessage = ref<string | null>(null);

    const sendLogin = async (request: LoginModel): Promise<boolean> => {
        try {
            const response = await authService.login(request);
            if (response.status === 200 && response.data.token) {
                errorMessage.value = null;
                return true;
            } else {
                errorMessage.value = 'We could not log you in. Please check your username/password and try again';
                return false;
            }
        } catch (error) {
            errorMessage.value = 'An unexpected error occurred';
            return false;
        }
    };

    const logout = () => {
        token.value = null;
        localStorage.removeItem('token');
    };
    
    const isAuthenticated = (): boolean => !!token.value;

    return {
        token,
        errorMessage,
        sendLogin,
        logout,
        isAuthenticated
    };
});
