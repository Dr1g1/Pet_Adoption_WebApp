import api from './axiosClient';
import type { JoinRequestReturnDto } from '../types/joinRequest.types';

export const joinRequestApi = {
    create: (shelterId: string, message: string) =>
        api.post('/joinRequest', { shelterId, message }).then(r => r.data),

    getMyRequests: () =>
        api.get<JoinRequestReturnDto[]>('/joinRequest/my').then(r => r.data),

    getPendingForShelter: (shelterId: string) =>
        api.get<JoinRequestReturnDto[]>(`/joinRequest/shelter/${shelterId}/pending`).then(r => r.data),

    approve: (requestId: string) =>
        api.patch('/joinRequest/approve', { requestId }).then(r => r.data),

    reject: (requestId: string) =>
        api.patch('/joinRequest/reject', { requestId }).then(r => r.data),

    cancel: (requestId: string) =>
        api.delete(`/joinRequest/${requestId}`).then(r => r.data)
};