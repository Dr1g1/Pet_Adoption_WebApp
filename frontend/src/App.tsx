import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useAuthStore } from './store/authStore';
import { Layout } from './components/Layout';
import { LoginPage } from './pages/auth/LoginPage';
import { RegisterPage } from './pages/auth/RegisterPage';
import { ProfilePage } from './pages/ProfilePage';
import { AnimalsPage } from './pages/AnimalsPage';
import { AnimalDetailPage } from './pages/AnimalDetailPage';
import { ShelterDashboard } from './pages/ShelterDashboard';
import { ShelterManagePage } from './pages/ShelterManagePage';
import { SheltersPage } from './pages/SheltersPage';
import { ShelterDetailPage } from './pages/ShelterDetailPage';
import { LikedAnimalsPage } from './pages/LikedAnimalsPage';

const queryClient = new QueryClient();

function PrivateRoute({
    children,
    allowedRoles
}: {
    children: React.ReactNode;
    allowedRoles?: Array<'User' | 'Volunteer'>;
}) {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    const role = useAuthStore(s => s.user?.role);

    if (!isAuthenticated) return <Navigate to="/login" replace />;
    if (allowedRoles && role && !allowedRoles.includes(role)) {
        return <Navigate to="/animals" replace />;
    }
    return <>{children}</>;
}

function AdminRoute({ children }: { children: React.ReactNode }) {
    const isAuthenticated = useAuthStore(s => s.isAuthenticated);
    const user = useAuthStore(s => s.user);

    if (!isAuthenticated) return <Navigate to="/login" replace />;
    if (user?.role !== 'Volunteer' || !user?.isAdmin) {
        return <Navigate to="/animals" replace />;
    }
    return <>{children}</>;
}

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    <Route
                        element={
                            <PrivateRoute>
                                <Layout />
                            </PrivateRoute>
                        }
                    >
                        <Route path="/" element={<Navigate to="/animals" replace />} />

                        <Route path="/animals" element={<AnimalsPage />} />
                        <Route path="/animals/:id" element={<AnimalDetailPage />} />

                        <Route path="/shelters" element={<SheltersPage />} />
                        <Route path="/shelters/:id" element={<ShelterDetailPage />} />

                        <Route path="/profile" element={
                            <PrivateRoute allowedRoles={['User']}>
                                <ProfilePage />
                            </PrivateRoute>
                        } />

                        <Route path="/liked" element={
                            <PrivateRoute allowedRoles={['User']}>
                                <LikedAnimalsPage />
                            </PrivateRoute>
                        } />

                        <Route path="/shelter" element={
                            <PrivateRoute allowedRoles={['Volunteer']}>
                                <ShelterDashboard />
                            </PrivateRoute>
                        } />

                        <Route path="/shelter/manage" element={
                            <AdminRoute>
                                <ShelterManagePage />
                            </AdminRoute>
                        } />
                    </Route>

                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </BrowserRouter>
        </QueryClientProvider>
    );
}