import api from './axiosClient';
import type { UserDto } from '../types/user.types';

export const userApi = {
    getById: (id: string) =>
        api.get<UserDto>(`/users/${id}`).then(r => r.data),

    update: (id: string, dto: Partial<UserDto>) =>
        api.put<UserDto>(`/users/${id}`, dto).then(r => r.data)
};