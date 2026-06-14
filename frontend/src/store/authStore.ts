import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { UserInfoDto } from '../types/auth.types';

interface AuthState {
    accessToken: string | null;
    refreshToken: string | null;
    user: UserInfoDto | null;
    isAuthenticated: boolean;

    login: (accessToken: string, refreshToken: string, user: UserInfoDto) => void;
    logout: () => void;
    setAccessToken: (token: string) => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            accessToken: null,
            refreshToken: null,
            user: null,
            isAuthenticated: false,

            login: (accessToken, refreshToken, user) =>
                set({ accessToken, refreshToken, user, isAuthenticated: true }),

            logout: () =>
                set({ accessToken: null, refreshToken: null, user: null, isAuthenticated: false }),

            setAccessToken: (accessToken) =>
                set({ accessToken })
        }),
        { name: 'auth' }
    )
);