export interface MedicalRecordResponseDto {
    id: string;
    description: string;
    date: string;
    clinicPhone: string;
    vetName: string;
    nextDueDate: string;
    vaccines: string[];
}

export interface MedicalRecordCreateDto {
    description: string;
    date: string;
    clinicPhone: string;
    vetName: string;
    nextDueDate: string;
    vaccines: string[];
}