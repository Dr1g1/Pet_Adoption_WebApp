import { useParams, useNavigate, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { shelterApi } from '../api/shelter.api';
import { animalApi } from '../api/animal.api';

export function ShelterDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const { data: shelter, isLoading: shelterLoading, error } = useQuery({
        queryKey: ['shelter', id],
        queryFn: () => shelterApi.getById(id!),
        enabled: !!id
    });

    const { data: animals, isLoading: animalsLoading } = useQuery({
        queryKey: ['animals', 'shelter', 'all', id],
        queryFn: () => animalApi.getAllByShelter(id!),
        enabled: !!id
    });

    if (shelterLoading) return <div className="max-w-4xl mx-auto p-8 text-gray-500">Učitavanje...</div>;
    if (error || !shelter) return <div className="max-w-4xl mx-auto p-8 text-red-500">Azil nije pronađen.</div>;

    return (
        <div className="max-w-4xl mx-auto p-8 space-y-6">
            <button onClick={() => navigate(-1)} className="text-sm text-gray-500 hover:text-gray-700">
                 Nazad
            </button>

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <h1 className="text-2xl font-bold text-gray-900">{shelter.name}</h1>
                <p className="text-gray-500 mt-1">{shelter.address}</p>

                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-6 text-sm">
                    <Stat label="Kapacitet" value={shelter.capacity?.toString() ?? '—'} />
                    <Stat label="Telefon" value={shelter.phone ?? '—'} />
                    <Stat label="Email" value={shelter.email ?? '—'} />
                    {shelter.rating != null && <Stat label="Ocena" value={shelter.rating.toFixed(1)} />}
                </div>

                {shelter.description && (
                    <p className="text-gray-600 text-sm mt-4">{shelter.description}</p>
                )}
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Životinje u azilu</h2>

                {animalsLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {!animalsLoading && animals?.length === 0 && (
                    <p className="text-gray-500 text-sm">Nema životinja.</p>
                )}

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {animals?.map(a => (
                        <Link
                            key={a.id}
                            to={`/animals/${a.id}`}
                            className={`border rounded-xl p-4 flex items-center gap-3 transition-shadow hover:shadow-md ${
                                a.isAdopted ? 'border-gray-200 bg-gray-50 opacity-75' : 'border-gray-100'
                            }`}
                        >
                            <div className="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center text-xl shrink-0">
                                {a.primaryImgUrl
                                    ? <img src={`/${a.primaryImgUrl}`} alt={a.name} className="w-full h-full object-cover rounded-lg" />
                                    : ''}
                            </div>
                            <div className="min-w-0">
                                <p className="font-medium text-gray-800 truncate">{a.name}</p>
                                <p className="text-xs text-gray-500 truncate">{a.breed}</p>
                                {a.isAdopted && (
                                    <span className="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full">Udomljen</span>
                                )}
                            </div>
                        </Link>
                    ))}
                </div>
            </div>
        </div>
    );
}

function Stat({ label, value }: { label: string; value: string }) {
    return (
        <div className="bg-gray-50 rounded-lg p-3">
            <p className="text-gray-400 text-xs">{label}</p>
            <p className="text-gray-900 font-semibold mt-0.5 truncate">{value}</p>
        </div>
    );
}