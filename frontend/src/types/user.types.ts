
export interface UserCreateDto {
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

export interface UserSummaryDto {
    id: string;
    name: string;
    surname: string;
    email: string;
    phone?: string;
    address: string;
}

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

export interface UserInfoDto {
    id: string;
    email: string;
    role: 'User' | 'Volunteer';
    shelterId?: string;
    isAdmin?: boolean;   
}