import api from './axiosClient';
import type { ShelterDto } from '../types/shelter.types';

export const shelterApi = {
    getById: (id: string) =>
        api.get<ShelterDto>(`/shelter/${id}`).then(r => r.data),

    getAll: () =>
        api.get<ShelterDto[]>('/shelter').then(r => r.data)
};