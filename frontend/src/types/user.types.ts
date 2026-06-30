
// Saljemo KA backendu pri kreiranju korisnika
export interface UserCreateDto {
    name: string;
    surname: string;
    email: string;
    password: string;
    phone?: string;
    bio?: string;
    address: string;
    hasChildren: boolean;
    hasPets: boolean;
    livingSpace?: string;
}

// Primamo SA backenda — skracena verzija za liste
export interface UserSummaryDto {
    id: string;
    name: string;
    surname: string;
    email: string;
    phone?: string;
    address: string;
}

// Primamo SA backenda — pun profil korisnika
export interface UserDto {
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
    likedAnimalIds?: string[];
    adoptedAnimalIds?: string[];
}

// Saljemo KA backendu pri izmeni - sva polja opciona
export interface UserUpdateDto {
    name?: string;
    surname?: string;
    phone?: string;
    bio?: string;
    address?: string;
    hasChildren?: boolean;
    hasPets?: boolean;
    livingSpace?: string;
}