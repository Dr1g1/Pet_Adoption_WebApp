import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { shelterApi } from '../api/shelter.api';
import { animalApi } from '../api/animal.api';
import { volunteerApi } from '../api/volunteer.api';
import { JoinShelterPage } from './joinShelterPage';

export function ShelterDashboard() {
    const shelterId = useAuthStore(s => s.user?.shelterId);
    const isAdmin = useAuthStore(s => s.user?.isAdmin);

    const { data: shelter, isLoading: shelterLoading } = useQuery({
        queryKey: ['shelter', shelterId],
        queryFn: () => shelterApi.getById(shelterId!),
        enabled: !!shelterId
    });

    const { data: animals, isLoading: animalsLoading } = useQuery({
        queryKey: ['animals', 'shelter', shelterId],
        queryFn: () => animalApi.getByShelter(shelterId!),
        enabled: !!shelterId
    });

    const { data: volunteers, isLoading: volunteersLoading } = useQuery({
        queryKey: ['volunteers', 'shelter', shelterId],
        queryFn: () => volunteerApi.getByShelter(shelterId!),
        enabled: !!shelterId
    });

    if (!shelterId) {
        return (
            <div className="max-w-4xl mx-auto p-8">
                <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-yellow-800">
                    return <JoinShelterPage/>
                </div>
            </div>
        );
    }

    if (shelterLoading) {
        return <div className="max-w-4xl mx-auto p-8 text-gray-500">Učitavanje azila...</div>;
    }

    return (
        <div className="max-w-4xl mx-auto p-8 space-y-6">

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <div className="flex items-start justify-between">
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900">{shelter?.name}</h1>
                        <p className="text-gray-500 mt-1">{shelter?.address}</p>
                    </div>
                    {isAdmin && (
                        <Link
                            to="/shelter/manage"
                            className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors"
                        >
                            Upravljanje azilom 
                        </Link>
                    )}
                </div>

                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-6 text-sm">
                    <Stat label="Kapacitet" value={shelter?.capacity?.toString() ?? '—'} />
                    <Stat label="Životinje" value={animals?.length?.toString() ?? '0'} />
                    <Stat label="Volonteri" value={volunteers?.length?.toString() ?? '0'} />
                    <Stat label="Telefon" value={shelter?.phone ?? '—'} />
                </div>

                {shelter?.description && (
                    <p className="text-gray-600 text-sm mt-4">{shelter.description}</p>
                )}
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                    Životinje u azilu
                </h2>
                {animalsLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {!animalsLoading && animals?.length === 0 && (
                    <p className="text-gray-500 text-sm">Nema životinja u azilu.</p>
                )}
                {!animalsLoading && animals && animals.length > 0 && (
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                        {animals.map(a => (
                            <Link
                                key={a.id}
                                to={`/animals/${a.id}`}
                                className="border border-gray-100 rounded-xl p-4 hover:shadow-md transition-shadow flex items-center gap-3"
                            >
                                <div className="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center text-xl shrink-0">
                                    {a.primaryImgUrl
                                        ? <img src={`/${a.primaryImgUrl}`} alt={a.name} className="w-full h-full object-cover rounded-lg" />
                                        : ''}
                                </div>
                                <div className="min-w-0">
                                    <p className="font-medium text-gray-800 truncate">{a.name}</p>
                                    <p className="text-xs text-gray-500 truncate">{a.breed}</p>
                                </div>
                            </Link>
                        ))}
                    </div>
                )}
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                    Volonteri azila
                </h2>
                {volunteersLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {!volunteersLoading && volunteers?.length === 0 && (
                    <p className="text-gray-500 text-sm">Nema volontera.</p>
                )}
                {!volunteersLoading && volunteers && volunteers.length > 0 && (
                    <div className="space-y-2">
                        {volunteers.map(v => (
                            <div key={v.id} className="flex items-center justify-between border border-gray-100 rounded-lg p-3">
                                <div>
                                    <p className="text-sm font-medium text-gray-800">
                                        {v.name} {v.surname}
                                    </p>
                                    <p className="text-xs text-gray-400">{v.email}</p>
                                </div>
                                <span className={`text-xs px-2 py-1 rounded-full font-medium ${
                                    v.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'
                                }`}>
                                    {v.isActive ? 'Aktivan' : 'Neaktivan'}
                                </span>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

function Stat({ label, value }: { label: string; value: string }) {
    return (
        <div className="bg-gray-50 rounded-lg p-3">
            <p className="text-gray-400 text-xs">{label}</p>
            <p className="text-gray-900 font-semibold mt-0.5">{value}</p>
        </div>
    );
}