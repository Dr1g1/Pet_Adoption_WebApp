import api from './axiosClient';
import type { LoginDto, RegisterDto, AuthResponseDto } from '../types/auth.types';

export const authApi = {
    login: (dto: LoginDto) =>
        api.post<AuthResponseDto>('/auth/login', dto).then(r => r.data),

    register: (dto: RegisterDto) =>
        api.post<AuthResponseDto>('/auth/register', dto).then(r => r.data),

    logout: (refreshToken: string) =>
        api.post('/auth/logout', JSON.stringify(refreshToken)),

    refresh: (refreshToken: string) =>
        api.post<AuthResponseDto>('/auth/refresh', JSON.stringify(refreshToken))
           .then(r => r.data),

    me: () => api.get('/auth/me').then(r => r.data)
};

