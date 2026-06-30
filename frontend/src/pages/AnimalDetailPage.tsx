import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { animalApi } from '../api/animal.api';
import { adoptionRequestApi } from '../api/adoptionRequest.api';
import { userApi } from '../api/user.api';
import { useAuthStore } from '../store/authStore';
import { AnimalMedicalView } from '../components/shelter/AnimalMedicalView';

const boolLabel: Record<string, string> = { Yes: 'Da', No: 'Ne', Unkown: 'Nepoznato' };

export function AnimalDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const userId = useAuthStore(s => s.user?.id);
    const role = useAuthStore(s => s.user?.role);

    const [message, setMessage] = useState('');
    const [sent, setSent] = useState(false);
    const [activeImg, setActiveImg] = useState(0);
    const [showMedical, setShowMedical] = useState(false);

    const { data: animal, isLoading, error } = useQuery({
        queryKey: ['animal', id],
        queryFn: () => animalApi.getById(id!),
        enabled: !!id
    });

    const { data: userProfile } = useQuery({
        queryKey: ['user', userId],
        queryFn: () => userApi.getById(userId!),
        enabled: !!userId && role === 'User'
    });

    const isLiked = userProfile?.likedAnimalIds?.includes(id!) ?? false;

    const likeMutation = useMutation({
        mutationFn: () => (isLiked ? animalApi.unlike(id!) : animalApi.like(id!)),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['user', userId] });
            queryClient.invalidateQueries({ queryKey: ['likedAnimals'] });
        }
    });

    const { data: likeCount } = useQuery({
        queryKey: ['likeCount', id],
        queryFn: () => animalApi.getLikeCount(id!),
        enabled: !!id && role === 'Volunteer'
    });

    const createRequest = useMutation({
        mutationFn: () => adoptionRequestApi.create(id!, message),
        onSuccess: () => {
            setSent(true);
            queryClient.invalidateQueries({ queryKey: ['adoptionRequests', 'my'] });
        }
    });

    if (isLoading) return <div className="max-w-3xl mx-auto p-8 text-gray-500">Učitavanje...</div>;
    if (error || !animal) return <div className="max-w-3xl mx-auto p-8 text-red-500">Životinja nije pronađena.</div>;

    const images = animal.images ?? [];
    const hasImages = images.length > 0;

    const prevImg = () => setActiveImg(i => (i === 0 ? images.length - 1 : i - 1));
    const nextImg = () => setActiveImg(i => (i === images.length - 1 ? 0 : i + 1));

    return (
        <div className="max-w-3xl mx-auto p-8">
            <button onClick={() => navigate(-1)} className="text-sm text-gray-500 hover:text-gray-700 mb-4">
                 Nazad
            </button>

            <div className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
                <div className="relative aspect-video bg-gray-100 flex items-center justify-center">
                    {hasImages ? (
                        <>
                            <img
                                src={`/${images[activeImg]}`}
                                alt={`${animal.name} ${activeImg + 1}`}
                                className="w-full h-full object-cover"
                            />
                            {images.length > 1 && (
                                <>
                                    <button
                                        onClick={prevImg}
                                        className="absolute left-3 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white
                                            w-9 h-9 rounded-full shadow flex items-center justify-center text-gray-700 transition-colors"
                                    >
                                        ‹
                                    </button>
                                    <button
                                        onClick={nextImg}
                                        className="absolute right-3 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white
                                            w-9 h-9 rounded-full shadow flex items-center justify-center text-gray-700 transition-colors"
                                    >
                                        ›
                                    </button>
                                    <span className="absolute bottom-3 right-3 bg-black/50 text-white text-xs px-2 py-1 rounded-full">
                                        {activeImg + 1} / {images.length}
                                    </span>
                                </>
                            )}
                        </>
                    ) : (
                        <span className="text-6xl"></span>
                    )}
                </div>

                {images.length > 1 && (
                    <div className="flex gap-2 p-3 overflow-x-auto border-b border-gray-100">
                        {images.map((img, idx) => (
                            <button
                                key={img}
                                onClick={() => setActiveImg(idx)}
                                className={`shrink-0 w-16 h-16 rounded-lg overflow-hidden border-2 transition-colors ${
                                    idx === activeImg ? 'border-orange-500' : 'border-transparent hover:border-gray-300'
                                }`}
                            >
                                <img src={`/${img}`} alt={`thumb ${idx + 1}`} className="w-full h-full object-cover" />
                            </button>
                        ))}
                    </div>
                )}

                <div className="p-6">
                    <div className="flex items-start justify-between">
                        <div>
                            <h1 className="text-2xl font-bold text-gray-900">{animal.name}</h1>
                            <p className="text-gray-500">{animal.species} · {animal.breed}</p>
                        </div>

                        {role === 'User' && (
                            <button
                                onClick={() => likeMutation.mutate()}
                                disabled={likeMutation.isPending}
                                className={`inline-flex items-center gap-2 text-sm font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50 ${
                                    isLiked
                                        ? 'bg-red-50 text-red-600 hover:bg-red-100'
                                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                                }`}
                            >
                                <span>{isLiked ? '♥' : '♡'}</span>
                                {isLiked ? 'Sviđa mi se' : 'Omiljeno'}
                            </button>
                        )}
                    </div>

                    {role === 'Volunteer' && likeCount && (
                        <p className="mt-2 text-sm text-gray-500">
                            ♥ {likeCount.count} {likeCount.count === 1 ? 'lajk' : 'lajkova'}
                        </p>
                    )}

                    <div className="grid grid-cols-2 gap-4 mt-6 text-sm">
                        <Info label="Pol" value={animal.gender === 'Male' ? 'Mužjak' : 'Ženka'} />
                        <Info label="Veličina" value={animal.size} />
                        {animal.age != null && <Info label="Starost" value={`${animal.age} god.`} />}
                        <Info label="Vakcinisan" value={boolLabel[animal.isVaccinated]} />
                        <Info label="Sterilisan" value={boolLabel[animal.isSterilized]} />
                    </div>

                    {animal.description && (
                        <div className="mt-6">
                            <p className="text-gray-400 text-sm">Opis</p>
                            <p className="text-gray-800 mt-1">{animal.description}</p>
                        </div>
                    )}

                    <button
                        onClick={() => setShowMedical(true)}
                        className="mt-4 text-sm text-orange-600 hover:text-orange-700 font-medium"
                    >
                        Pogledaj medicinske kartone 
                    </button>

                    {role === 'User' && (
                        <div className="mt-8 border-t border-gray-100 pt-6">
                            {sent ? (
                                <div className="p-4 bg-green-50 border border-green-200 rounded-lg text-green-700 text-sm">
                                    Zahtev je uspešno poslat! Možeš ga pratiti na svom profilu.
                                </div>
                            ) : (
                                <>
                                    <h2 className="font-semibold text-gray-900 mb-2">Pošalji zahtev za usvajanje</h2>
                                    <textarea
                                        value={message}
                                        onChange={e => setMessage(e.target.value)}
                                        placeholder="Poruka azilu (zašto želiš da usvojiš ovu životinju)..."
                                        rows={3}
                                        className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm
                                            focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none"
                                    />
                                    {createRequest.isError && (
                                        <p className="text-sm text-red-500 mt-1">Greška pri slanju zahteva.</p>
                                    )}
                                    <button
                                        onClick={() => createRequest.mutate()}
                                        disabled={createRequest.isPending}
                                        className="mt-3 bg-orange-500 hover:bg-orange-600 text-white font-medium
                                            px-5 py-2.5 rounded-lg transition-colors disabled:opacity-50"
                                    >
                                        {createRequest.isPending ? 'Slanje...' : 'Pošalji zahtev'}
                                    </button>
                                </>
                            )}
                        </div>
                    )}
                </div>
            </div>

            {showMedical && (
                <AnimalMedicalView
                    animalId={animal.id}
                    animalName={animal.name}
                    canEdit={false}
                    onClose={() => setShowMedical(false)}
                />
            )}
        </div>
    );
}

function Info({ label, value }: { label: string; value: string }) {
    return (
        <div>
            <p className="text-gray-400">{label}</p>
            <p className="text-gray-800">{value}</p>
        </div>
    );
}