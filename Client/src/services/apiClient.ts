import axios from 'axios';

const apiClient = axios.create({
    baseURL: 'https://localhost:5001/',
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

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
