
// Odgovara ShelterDto.cs
export interface ShelterDto {
    id: string;
    name: string;
    address: string;
    phone: string;
    email: string;
    capacity: number;         // int → number
    rating?: number;          // float? → number?
    description?: string;
}