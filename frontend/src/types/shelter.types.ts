export interface ShelterDto {
    id: string;
    name: string;
    address: string;
    phone: string;
    email: string;
    capacity: number;
    rating?: number;
    description?: string;
}