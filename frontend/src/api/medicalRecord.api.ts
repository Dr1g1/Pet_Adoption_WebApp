import api from './axiosClient';
import type { MedicalRecordResponseDto, MedicalRecordCreateDto } from '../types/medicalRecord.types';

export const medicalRecordApi = {
    getForAnimal: (animalId: string) =>
        api.get<MedicalRecordResponseDto[]>(`/medicalRecord/${animalId}/medicalrecords`).then(r => r.data),

    create: (animalId: string, dto: MedicalRecordCreateDto) =>
        api.post<MedicalRecordResponseDto>(`/medicalRecord/${animalId}`, dto).then(r => r.data),

    delete: (recordId: string) =>
        api.delete(`/medicalRecord/${recordId}`).then(r => r.data)
};