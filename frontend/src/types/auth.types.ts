export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    name: string;
    surname: string;
    email: string;
    password: string;
    confirmPassword: string;
    phone?: string;
    bio?: string;
    address: string;
    hasChildren: boolean;
    hasPets: boolean;
    livingSpace?: string;
    role: 'User' | 'Volunteer';
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}

export interface UserInfoDto {
    id: string;
    email: string;
    role: 'User' | 'Volunteer';
    shelterId?: string;
    isAdmin?:boolean;
}

export interface AuthResponseDto {
    accessToken: string;
    refreshToken: string;
    accessTokenExpiry: string;
    userInfo: UserInfoDto;
}