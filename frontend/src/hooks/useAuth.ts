import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/auth.api';
import { useAuthStore } from '../store/authStore';
import type { LoginDto, RegisterDto } from '../types/auth.types';

export function useLogin() {
    const login = useAuthStore(s => s.login);
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (dto: LoginDto) => authApi.login(dto),
        onSuccess: (data) => {
            login(data.accessToken, data.refreshToken, data.userInfo);
            navigate('/');
        }
    });
}

export function useRegister() {
    const login = useAuthStore(s => s.login);
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (dto: RegisterDto) => authApi.register(dto),
        onSuccess: (data) => {
            login(data.accessToken, data.refreshToken, data.userInfo);
            navigate('/');
        }
    });
}

export function useLogout() {
    const { logout, refreshToken } = useAuthStore();
    const navigate = useNavigate();

    return () => {
        if (refreshToken) authApi.logout(refreshToken);
        logout();
        navigate('/login');
    };
}