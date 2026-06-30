import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { animalApi } from '../api/animal.api';

export function LikedAnimalsPage() {
    const { data: animals, isLoading, error } = useQuery({
        queryKey: ['likedAnimals'],
        queryFn: () => animalApi.getLiked()
    });

    if (isLoading) return <div className="max-w-5xl mx-auto p-8 text-gray-500">Učitavanje...</div>;
    if (error) return <div className="max-w-5xl mx-auto p-8 text-red-500">Greška pri učitavanju.</div>;

    return (
        <div className="max-w-5xl mx-auto p-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Omiljene životinje</h1>

            {animals?.length === 0 && (
                <p className="text-gray-500">Još nisi dodao/la nijednu životinju u omiljene.</p>
            )}

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
                {animals?.map(a => (
                    <Link
                        key={a.id}
                        to={`/animals/${a.id}`}
                        className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden hover:shadow-md transition-shadow"
                    >
                        <div className="aspect-4/3 bg-gray-100 flex items-center justify-center">
                            {a.primaryImgUrl
                                ? <img src={`/${a.primaryImgUrl}`} alt={a.name} className="w-full h-full object-cover" />
                                : <span className="text-5xl"></span>}
                        </div>
                        <div className="p-4">
                            <h3 className="font-semibold text-gray-900">{a.name}</h3>
                            <p className="text-sm text-gray-500">{a.breed}</p>
                        </div>
                    </Link>
                ))}
            </div>
        </div>
    );
}