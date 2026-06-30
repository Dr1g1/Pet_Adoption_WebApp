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
    //Volunteer-specificna polja:
    skills?: string[];
    availableDays?: string[];
    shelterId?: string;
}

export interface UserInfoDto {
    id: string;
    email: string;
    role: 'User' | 'Volunteer' | 'Admin';
    shelterId?: string;
}

export interface AuthResponseDto {
    accessToken: string;
    refreshToken: string;
    accessTokenExpiry: string;
    userInfo: UserInfoDto;
}