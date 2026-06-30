import type { UserCreateDto, UserUpdateDto } from './user.types';
import type { ShelterDto } from './shelter.types';

export interface VolunteerCreateDto extends UserCreateDto {
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}

export interface VolunteerSummaryDto {
    id: string;
    name: string;
    surname: string;
    email: string;
    phone?: string;
    address: string;
    isActive: boolean;
    rating?: number;
    shelterName?: string;
}

export interface VolunteerDto {
    id: string;
    name: string;
    surname: string;
    email: string;
    phone?: string;
    bio?: string;
    address: string;
    hasChildren: boolean;
    hasPets: boolean;
    livingSpace?: string;
    isAdmin: boolean;
    isActive: boolean;
    rating?: number;
    skills?: string[];
    availableDays?: string[];
    joinedAt?: string;
    shelter?: ShelterDto;
}

export interface VolunteerUpdateDto extends UserUpdateDto {
    isActive?: boolean;
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}