import { useQuery } from '@tanstack/react-query';
import { userApi } from '../../api/user.api';

export function UserProfileView({ userId, onClose }: { userId: string; onClose: () => void }) {
    const { data: user, isLoading, error } = useQuery({
        queryKey: ['user', userId],
        queryFn: () => userApi.getById(userId)
    });

    return (
        <div
            className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50"
            onClick={onClose}
        >
            <div
                className="bg-white rounded-2xl shadow-xl max-w-md w-full p-6"
                onClick={e => e.stopPropagation()}
            >
                <div className="flex items-center justify-between mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Profil tražioca</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
                </div>

                {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {error && <p className="text-red-500 text-sm">Greška pri učitavanju profila.</p>}

                {user && (
                    <div className="space-y-3 text-sm">
                        <div>
                            <p className="text-xl font-bold text-gray-900">{user.name} {user.surname}</p>
                            <p className="text-gray-500">{user.email}</p>
                        </div>
                        <div className="grid grid-cols-2 gap-3 pt-2 border-t border-gray-100">
                            <Field label="Telefon" value={user.phone || '—'} />
                            <Field label="Adresa" value={user.address} />
                            <Field label="Ima decu" value={user.hasChildren ? 'Da' : 'Ne'} />
                            <Field label="Ima ljubimce" value={user.hasPets ? 'Da' : 'Ne'} />
                            {user.livingSpace && <Field label="Prostor" value={user.livingSpace} span />}
                        </div>
                        {user.bio && (
                            <div className="pt-2 border-t border-gray-100">
                                <p className="text-gray-400 text-xs">O tražiocu</p>
                                <p className="text-gray-800 mt-1">{user.bio}</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
}

function Field({ label, value, span }: { label: string; value: string; span?: boolean }) {
    return (
        <div className={span ? 'col-span-2' : ''}>
            <p className="text-gray-400 text-xs">{label}</p>
            <p className="text-gray-800">{value}</p>
        </div>
    );
}