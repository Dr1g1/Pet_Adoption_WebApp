import { useQuery } from '@tanstack/react-query';
import { volunteerApi } from '../../api/volunteer.api';

export function VolunteerProfileView({ volunteerId, onClose }: { volunteerId: string; onClose: () => void }) {
    const { data: v, isLoading, error } = useQuery({
        queryKey: ['volunteer', volunteerId],
        queryFn: () => volunteerApi.getById(volunteerId)
    });

    return (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-white rounded-2xl shadow-xl max-w-md w-full p-6" onClick={e => e.stopPropagation()}>
                <div className="flex items-center justify-between mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Profil volontera</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
                </div>

                {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {error && <p className="text-red-500 text-sm">Greška pri učitavanju.</p>}

                {v && (
                    <div className="space-y-3 text-sm">
                        <div>
                            <p className="text-xl font-bold text-gray-900">
                                {v.name} {v.surname}
                                {v.isAdmin && (
                                    <span className="ml-2 text-xs bg-orange-100 text-orange-700 px-2 py-1 rounded-full">Admin</span>
                                )}
                            </p>
                            <p className="text-gray-500">{v.email}</p>
                        </div>
                        <div className="grid grid-cols-2 gap-3 pt-2 border-t border-gray-100">
                            <Field label="Telefon" value={v.phone || '—'} />
                            <Field label="Adresa" value={v.address} />
                            <Field label="Status" value={v.isActive ? 'Aktivan' : 'Neaktivan'} />
                            {v.rating != null && <Field label="Ocena" value={v.rating.toFixed(1)} />}
                        </div>
                        {v.skills && v.skills.length > 0 && (
                            <div className="pt-2 border-t border-gray-100">
                                <p className="text-gray-400 text-xs mb-1">Veštine</p>
                                <div className="flex flex-wrap gap-1">
                                    {v.skills.map(s => (
                                        <span key={s} className="text-xs bg-gray-100 px-2 py-1 rounded-full">{s}</span>
                                    ))}
                                </div>
                            </div>
                        )}
                        {v.bio && (
                            <div className="pt-2 border-t border-gray-100">
                                <p className="text-gray-400 text-xs">O volonteru</p>
                                <p className="text-gray-800 mt-1">{v.bio}</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
}

function Field({ label, value }: { label: string; value: string }) {
    return (
        <div>
            <p className="text-gray-400 text-xs">{label}</p>
            <p className="text-gray-800">{value}</p>
        </div>
    );
}