
// Odgovara MedicalRecordCreateDto.cs
export interface MedicalRecordCreateDto {
    description: string;
    date: string;             // DateTime → string
    clinicPhone: string;
    vetName: string;
    nextDueDate: string;      // DateTime → string
    vaccines?: string[];
}

// Odgovara MedicalRecordResponseDto.cs 
export interface MedicalRecordResponseDto {
    id: string;
    description: string;
    date: string;
    clinicPhone: string;
    vetName: string;
    nextDueDate: string;
    vaccines?: string[];
}