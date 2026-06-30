import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuthStore } from '../../store/authStore';
import { volunteerApi } from '../../api/volunteer.api';
import { VolunteerProfileView } from './VolunteerProfileView';

export function VolunteersManageTab({ shelterId }: { shelterId: string }) {
    const queryClient = useQueryClient();
    const myId = useAuthStore(s => s.user?.id);
    const [viewingId, setViewingId] = useState<string | null>(null);

    const { data: volunteers, isLoading } = useQuery({
        queryKey: ['volunteers', 'shelter', shelterId],
        queryFn: () => volunteerApi.getByShelter(shelterId)
    });

    const setAdmin = useMutation({
        mutationFn: ({ id, isAdmin }: { id: string; isAdmin: boolean }) =>
            volunteerApi.setAdmin(id, isAdmin),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['volunteers', 'shelter', shelterId] })
    });

    const removeFromShelter = useMutation({
        mutationFn: (id: string) => volunteerApi.removeFromShelter(id),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['volunteers', 'shelter', shelterId] })
    });

    if (isLoading) return <p className="text-gray-500 text-sm">Učitavanje...</p>;
    if (!volunteers || volunteers.length === 0) {
        return <p className="text-gray-500 text-sm">Nema volontera u azilu.</p>;
    }

    return (
        <div className="space-y-2">
            <p className="text-sm text-gray-500 mb-3">
                Možeš dodeliti admin privilegije ili ukloniti volontera iz azila.
            </p>
            {volunteers.map(v => (
                <div key={v.id} className="flex items-center justify-between border border-gray-100 rounded-lg p-3">
                    <div>
                        <button
                            onClick={() => setViewingId(v.id)}
                            className="text-sm font-medium text-gray-800 hover:text-orange-600 text-left"
                        >
                            {v.name} {v.surname}
                            {v.id === myId && <span className="text-xs text-gray-400 ml-2">(ti)</span>}
                        </button>
                        <p className="text-xs text-gray-400">{v.email}</p>
                    </div>

                    {v.id !== myId && (
                        <div className="flex items-center gap-2">
                            <button
                                onClick={() => setAdmin.mutate({ id: v.id, isAdmin: true })}
                                disabled={setAdmin.isPending || removeFromShelter.isPending}
                                className="text-xs bg-orange-100 text-orange-700 hover:bg-orange-200 px-3 py-1.5 rounded-lg font-medium transition-colors disabled:opacity-50"
                            >
                                Postavi za admina
                            </button>
                            <button
                                onClick={() => {
                                    if (confirm(`Ukloniti ${v.name} ${v.surname} iz azila?`))
                                        removeFromShelter.mutate(v.id);
                                }}
                                disabled={setAdmin.isPending || removeFromShelter.isPending}
                                className="text-xs text-red-500 hover:text-red-700 px-2 py-1.5 disabled:opacity-50"
                            >
                                Ukloni
                            </button>
                        </div>
                    )}
                </div>
            ))}

            {viewingId && (
                <VolunteerProfileView volunteerId={viewingId} onClose={() => setViewingId(null)} />
            )}
        </div>
    );
}