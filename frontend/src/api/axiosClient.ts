import axios from 'axios';
import { useAuthStore } from '../store/authStore';

const api = axios.create({
    baseURL: '/api',
    headers: { 'Content-Type': 'application/json' }
});

//dodajemo jwt token za svaki zahtev
api.interceptors.request.use(config => {
    const token = useAuthStore.getState().accessToken;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

let isRefreshing = false;
let pendingQueue: Array<() => void> = [];

api.interceptors.response.use(
    response => response,
    async error => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            const { refreshToken, logout, setAccessToken } = useAuthStore.getState();
            if (!refreshToken) {
                logout();
                return Promise.reject(error);
            }

            if (isRefreshing) {
                return new Promise(resolve => {
                    pendingQueue.push(() => resolve(api(originalRequest)));
                });
            }

            isRefreshing = true;
            try {
                const { data } = await axios.post('/api/auth/refresh', JSON.stringify(refreshToken), {
                    headers: { 'Content-Type': 'application/json' }
                });
                setAccessToken(data.accessToken);
                pendingQueue.forEach(cb => cb());
                pendingQueue = [];
                return api(originalRequest);
            } catch (refreshError) {
                logout();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

export default api;