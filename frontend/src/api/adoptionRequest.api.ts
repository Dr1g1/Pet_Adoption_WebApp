import api from './axiosClient';
import type { AdoptionRequestReturnDto } from '../types/adoptionRequest.types';

export const adoptionRequestApi = {
    getMyRequests: () =>
        api.get<AdoptionRequestReturnDto[]>('/adoptionRequest/my').then(r => r.data),

    create: (animalId: string, message: string) =>
        api.post('/adoptionRequest', { animalId, message }).then(r => r.data),

    delete: (requestId: string) =>
        api.delete(`/adoptionRequest/${requestId}`).then(r => r.data),
    getPendingForShelter: (shelterId: string) =>
        api.get<AdoptionRequestReturnDto[]>(`/adoptionRequest/shelter/${shelterId}/pending`).then(r => r.data),

    approve: (requestId: string, responseMessage: string) =>
        api.patch('/adoptionRequest/approve', { requestId, responseMessage }).then(r => r.data),

    reject: (requestId: string, responseMessage: string) =>
        api.patch('/adoptionRequest/reject', { requestId, responseMessage }).then(r => r.data)
};