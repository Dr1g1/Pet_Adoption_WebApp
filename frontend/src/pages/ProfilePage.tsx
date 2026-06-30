import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuthStore } from '../store/authStore';
import { userApi } from '../api/user.api';
import { adoptionRequestApi } from '../api/adoptionRequest.api';
import type { AdoptionRequestStatus } from '../types/adoptionRequest.types';
import type { UserDto } from '../types/user.types';

const statusLabel: Record<AdoptionRequestStatus, string> = {
    Pending: 'Na čekanju', Approved: 'Prihvaćen', Rejected: 'Odbijen'
};
const statusClass: Record<AdoptionRequestStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-700',
    Approved: 'bg-green-100 text-green-700',
    Rejected: 'bg-red-100 text-red-700'
};

export function ProfilePage() {
    const userId = useAuthStore(s => s.user?.id);
    const queryClient = useQueryClient();
    const [editing, setEditing] = useState(false);
    const [form, setForm] = useState<Partial<UserDto>>({});

    const { data: profile, isLoading, error } = useQuery({
        queryKey: ['user', userId],
        queryFn: () => userApi.getById(userId!),
        enabled: !!userId
    });

    const { data: requests, isLoading: requestsLoading } = useQuery({
        queryKey: ['adoptionRequests', 'my'],
        queryFn: () => adoptionRequestApi.getMyRequests(),
        enabled: !!userId
    });

    const updateProfile = useMutation({
        mutationFn: () => userApi.update(userId!, form),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['user', userId] });
            setEditing(false);
        }
    });

    const cancelRequest = useMutation({
        mutationFn: (requestId: string) => adoptionRequestApi.delete(requestId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['adoptionRequests', 'my'] })
    });

    if (isLoading) return <div className="max-w-2xl mx-auto p-8 text-gray-500">Učitavanje profila...</div>;
    if (error || !profile) return <div className="max-w-2xl mx-auto p-8 text-red-500">Greška pri učitavanju profila.</div>;

    const startEdit = () => {
        setForm({
            name: profile.name, surname: profile.surname, phone: profile.phone,
            bio: profile.bio, address: profile.address,
            hasChildren: profile.hasChildren, hasPets: profile.hasPets,
            livingSpace: profile.livingSpace
        });
        setEditing(true);
    };

    return (
        <div className="max-w-2xl mx-auto p-8 space-y-6">

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl font-bold text-gray-900">
                        {profile.name} {profile.surname}
                    </h1>
                    {!editing && (
                        <button onClick={startEdit}
                            className="text-sm text-orange-600 hover:text-orange-700 font-medium">
                            Izmeni
                        </button>
                    )}
                </div>

                {!editing ? (
                    <div className="grid grid-cols-2 gap-4 text-sm">
                        <Info label="Email" value={profile.email} />
                        <Info label="Telefon" value={profile.phone || '—'} />
                        <Info label="Adresa" value={profile.address} span />
                        {profile.bio && <Info label="O meni" value={profile.bio} span />}
                        <Info label="Ima decu" value={profile.hasChildren ? 'Da' : 'Ne'} />
                        <Info label="Ima ljubimce" value={profile.hasPets ? 'Da' : 'Ne'} />
                        {profile.livingSpace && <Info label="Prostor" value={profile.livingSpace} span />}
                    </div>
                ) : (
                    <div className="space-y-4">
                        <div className="grid grid-cols-2 gap-4">
                            <EditField label="Ime" value={form.name ?? ''}
                                onChange={v => setForm(f => ({ ...f, name: v }))} />
                            <EditField label="Prezime" value={form.surname ?? ''}
                                onChange={v => setForm(f => ({ ...f, surname: v }))} />
                            <EditField label="Telefon" value={form.phone ?? ''}
                                onChange={v => setForm(f => ({ ...f, phone: v }))} />
                            <EditField label="Adresa" value={form.address ?? ''}
                                onChange={v => setForm(f => ({ ...f, address: v }))} />
                        </div>
                        <EditField label="O meni" value={form.bio ?? ''}
                            onChange={v => setForm(f => ({ ...f, bio: v }))} />
                        <div className="flex gap-4">
                            <label className="flex items-center gap-2 text-sm text-gray-600">
                                <input type="checkbox" checked={form.hasChildren ?? false}
                                    onChange={e => setForm(f => ({ ...f, hasChildren: e.target.checked }))}
                                    className="w-4 h-4 accent-orange-500" />
                                Imam decu
                            </label>
                            <label className="flex items-center gap-2 text-sm text-gray-600">
                                <input type="checkbox" checked={form.hasPets ?? false}
                                    onChange={e => setForm(f => ({ ...f, hasPets: e.target.checked }))}
                                    className="w-4 h-4 accent-orange-500" />
                                Imam ljubimce
                            </label>
                        </div>
                        <div className="flex gap-3 pt-2">
                            <button onClick={() => updateProfile.mutate()} disabled={updateProfile.isPending}
                                className="bg-orange-500 hover:bg-orange-600 text-white font-medium px-5 py-2 rounded-lg transition-colors disabled:opacity-50">
                                {updateProfile.isPending ? 'Čuvanje...' : 'Sačuvaj'}
                            </button>
                            <button onClick={() => setEditing(false)}
                                className="text-gray-600 hover:bg-gray-100 px-5 py-2 rounded-lg transition-colors">
                                Otkaži
                            </button>
                        </div>
                    </div>
                )}
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Moji zahtevi za usvajanje</h2>

                {requestsLoading && <p className="text-gray-500 text-sm">Učitavanje zahteva...</p>}
                {!requestsLoading && requests?.length === 0 && (
                    <p className="text-gray-500 text-sm">Još nisi poslao/la nijedan zahtev.</p>
                )}

                {!requestsLoading && requests && requests.length > 0 && (
                    <div className="space-y-3">
                        {requests.map(req => (
                            <div key={req.requestId}
                                className="flex items-center justify-between border border-gray-100 rounded-lg p-3">
                                <div>
                                    <p className="text-sm font-medium text-gray-800">{req.animalName}</p>
                                    <p className="text-xs text-gray-400">
                                        Poslato: {new Date(req.createdAt).toLocaleDateString('sr-RS')}
                                    </p>
                                </div>
                                <div className="flex items-center gap-3">
                                    <span className={`text-xs font-medium px-2.5 py-1 rounded-full ${statusClass[req.status]}`}>
                                        {statusLabel[req.status]}
                                    </span>
                                    {req.status === 'Pending' && (
                                        <button
                                            onClick={() => cancelRequest.mutate(req.requestId)}
                                            disabled={cancelRequest.isPending}
                                            className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
                                        >
                                            Otkaži
                                        </button>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

function Info({ label, value, span }: { label: string; value: string; span?: boolean }) {
    return (
        <div className={span ? 'col-span-2' : ''}>
            <p className="text-gray-400">{label}</p>
            <p className="text-gray-800">{value}</p>
        </div>
    );
}

function EditField({ label, value, onChange }: { label: string; value: string; onChange: (v: string) => void }) {
    return (
        <div>
            <label className="block text-sm text-gray-400 mb-1">{label}</label>
            <input value={value} onChange={e => onChange(e.target.value)}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm
                    focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none" />
        </div>
    );
}