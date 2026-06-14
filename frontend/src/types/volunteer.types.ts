import type { UserCreateDto, UserDto, UserSummaryDto, UserUpdateDto } from './user.types';
import type { ShelterDto } from './shelter.types';

// Nasledjuje sva User polja + dodaje volonterska
export interface VolunteerCreateDto extends UserCreateDto {
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}

// Pun profil volontera
export interface VolunteerDto extends UserDto {
    isAdmin: boolean;
    isActive: boolean;
    rating?: number;          // float? → number?
    skills?: string[];
    availableDays?: string[];
    joinedAt?: string;        // DateTime? → string?
    shelter?: ShelterDto;
}

// Skracena verzija za liste
export interface VolunteerSummaryDto extends UserSummaryDto {
    isActive: boolean;
    rating?: number;
    shelterName?: string;
}

// Izmena volontera
export interface VolunteerUpdateDto extends UserUpdateDto {
    isActive?: boolean;
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}