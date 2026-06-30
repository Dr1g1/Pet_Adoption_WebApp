import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useLogout } from '../hooks/useAuth';

export function Layout() {
    const user = useAuthStore(s => s.user);
    const logout = useLogout();
    const location = useLocation();

    const navLinks: { to: string; label: string }[] = [
        { to: '/animals', label: 'Životinje' },
        { to: '/shelters', label: 'Azili' }
    ];

    if (user?.role === 'User') {
        navLinks.push({ to: '/liked', label: 'Omiljene' });
        navLinks.push({ to: '/profile', label: 'Moj profil' });
    }

    if (user?.role === 'Volunteer') {
        navLinks.push({ to: '/shelter', label: 'Moj azil' });
        if (user.isAdmin) {
            navLinks.push({ to: '/shelter/manage', label: 'Upravljanje azilom' });
        }
    }

    const isActive = (to: string) =>
        location.pathname === to || location.pathname.startsWith(to + '/');

    return (
        <div className="min-h-screen bg-gray-50">
            <header className="bg-white border-b border-gray-100 sticky top-0 z-10">
                <div className="max-w-5xl mx-auto px-4 h-16 flex items-center justify-between">

                    <Link to="/animals" className="flex items-center gap-2">
                        <span className="text-2xl"></span>
                        <span className="font-bold text-gray-900">PetAdoption</span>
                    </Link>

                    <nav className="flex items-center gap-1">
                        {navLinks.map(link => (
                            <Link
                                key={link.to}
                                to={link.to}
                                className={`px-3 py-2 rounded-lg text-sm font-medium transition-colors
                                    ${isActive(link.to)
                                        ? 'bg-orange-50 text-orange-700'
                                        : 'text-gray-600 hover:bg-gray-100'}`}
                            >
                                {link.label}
                            </Link>
                        ))}

                        {user?.isAdmin && (
                            <span className="ml-2 text-xs bg-orange-100 text-orange-700 px-2 py-1 rounded-full font-medium">
                                Admin
                            </span>
                        )}

                        <button
                            onClick={logout}
                            className="ml-2 px-3 py-2 rounded-lg text-sm font-medium
                                text-gray-600 hover:bg-red-50 hover:text-red-600 transition-colors"
                        >
                            Odjava
                        </button>
                    </nav>
                </div>
            </header>

            <main>
                <Outlet />
            </main>
        </div>
    );
}