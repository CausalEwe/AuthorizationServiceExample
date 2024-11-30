import axios from 'axios';

const apiClient = axios.create({
    baseURL: '/',
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

const handleErrorResponse = (error: any) => {
    if (!error.response) {
        alert('Network error. Please check your internet connection.');
    } else {
        if (error.response.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        } else if (error.response.status === 403) {
            alert('You do not have permission to perform this action.');
        } else if (error.response.status === 500) {
            alert('An error occurred on the server. Please try again later.');
        }
    }
    return Promise.reject(error);
};

apiClient.interceptors.response.use(
    (response) => response,
    handleErrorResponse
);

export default apiClient;
