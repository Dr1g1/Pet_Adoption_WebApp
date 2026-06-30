import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useAuthStore } from './store/authStore';
import { LoginPage } from './pages/auth/LoginPage';
import { RegisterPage } from './pages/auth/RegisterPage';

const queryClient = new QueryClient();

function PrivateRoute({ children }: { children: React.ReactNode }) {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    {/* Zaštićene rute — dodaješ kasnije */}
                    <Route path="/" element={
                        <PrivateRoute>
                            <div className="p-8 text-center">
                                <h1 className="text-2xl font-bold">Početna stranica</h1>
                                <p className="text-gray-500 mt-2">Uspešno si se prijavila!</p>
                            </div>
                        </PrivateRoute>
                    } />
                </Routes>
            </BrowserRouter>
        </QueryClientProvider>
    );
}