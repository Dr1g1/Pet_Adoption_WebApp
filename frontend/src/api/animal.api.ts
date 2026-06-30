import api from './axiosClient';
import type { AnimalResponseDto, AnimalDetailResponseDto } from '../types/animal.types';

export interface AnimalCreatePayload {
    name: string;
    species: string;
    breed: string;
    age?: number;
    gender: 'Female' | 'Male';
    size: 'Small' | 'Medium' | 'Large';
    isVaccinated: 'Unknown' | 'Yes' | 'No';
    isSterilized: 'Unknown' | 'Yes' | 'No';
    isGoodWithKids: 'Unknown' | 'Yes' | 'No';
    isGoodWithPets: 'Unknown' | 'Yes' | 'No';
    description?: string;
    arrivedAt: string;
    shelterId: string;
    caretakerId: string;
}

export interface AnimalUpdatePayload {
    name?: string;
    species?: string;
    breed?: string;
    age?: number;
    gender?: 'Female' | 'Male';
    size?: 'Small' | 'Medium' | 'Large';
    isVaccinated?: 'Unknown' | 'Yes' | 'No';
    isSterilized?: 'Unknown' | 'Yes' | 'No';
    isGoodWithKids?: 'Unknown' | 'Yes' | 'No';
    isGoodWithPets?: 'Unknown' | 'Yes' | 'No';
    description?: string;
}

export const animalApi = {
    getAllAvailable: () =>
        api.get<AnimalResponseDto[]>('/animal/all').then(r => r.data),

    getById: (animalId: string) =>
        api.get<AnimalDetailResponseDto>(`/animal/${animalId}`).then(r => r.data),

    getByShelter: (shelterId: string) =>
        api.get<AnimalResponseDto[]>(`/animal?shelterId=${shelterId}`).then(r => r.data),

    create: (payload: AnimalCreatePayload) =>
        api.post('/animal', payload).then(r => r.data),

    delete: (animalId: string) =>
        api.delete(`/animal/${animalId}`).then(r => r.data),


    uploadImage: (animalId: string, file: File) => {
        const formData = new FormData();
        formData.append('file', file);
        return api.post(`/animal/${animalId}/images2`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        }).then(r => r.data);
    },

    deleteImage: (animalId: string, fileName: string) =>
        api.delete(`/animal/${animalId}/images/${fileName}`).then(r => r.data),

    getAllByShelter: (shelterId: string) =>
        api.get<AnimalResponseDto[]>(`/animal/shelter/${shelterId}/all`).then(r => r.data),

    update: (animalId: string, payload: AnimalUpdatePayload) =>
        api.patch(`/animal/${animalId}`, payload).then(r => r.data),

    like: (animalId: string) =>
        api.post(`/animal/${animalId}/like`).then(r => r.data),

    unlike: (animalId: string) =>
        api.delete(`/animal/${animalId}/like`).then(r => r.data),

    getLiked: () =>
        api.get<AnimalResponseDto[]>('/animal/liked').then(r => r.data),

    getLikeCount: (animalId: string) =>
        api.get<{ count: number }>(`/animal/${animalId}/likes/count`).then(r => r.data)

};