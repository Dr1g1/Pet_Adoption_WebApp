import api from './axiosClient';
import type { VolunteerSummaryDto, VolunteerDto } from '../types/volunteer.types';

export const volunteerApi = {
    getByShelter: (shelterId: string) =>
        api.get<VolunteerSummaryDto[]>(`/volunteers/by-shelter/${shelterId}`).then(r => r.data),

    setAdmin: (volunteerId: string, isAdmin: boolean) =>
        api.patch(`/volunteers/${volunteerId}/set-admin`, { isAdmin }).then(r => r.data),

    removeFromShelter: (volunteerId: string) =>
    api.delete(`/volunteers/${volunteerId}/remove-from-shelter`).then(r => r.data),

    getById: (id: string) =>
    api.get<VolunteerDto>(`/volunteers/${id}`).then(r => r.data),
};