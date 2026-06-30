import { useRef, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { animalApi } from '../../api/animal.api';

export function AnimalImagesView({ animalId, animalName, onClose }: {
    animalId: string;
    animalName: string;
    onClose: () => void;
}) {
    const queryClient = useQueryClient();
    const fileInput = useRef<HTMLInputElement>(null);
    const [uploadError, setUploadError] = useState('');

    const { data: animal, isLoading } = useQuery({
        queryKey: ['animal', animalId],
        queryFn: () => animalApi.getById(animalId)
    });

    const upload = useMutation({
        mutationFn: (file: File) => animalApi.uploadImage(animalId, file),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['animal', animalId] });
            setUploadError('');
        },
        onError: () => setUploadError('Greška pri otpremanju. Proveri format (jpg/png/webp) i veličinu (max 5MB).')
    });

    const deleteImg = useMutation({
        mutationFn: (fileName: string) => animalApi.deleteImage(animalId, fileName),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['animal', animalId] })
    });

    const getFileName = (path: string) => path.split(/[\\/]/).pop() ?? path;

    const handleFile = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) upload.mutate(file);
        if (fileInput.current) fileInput.current.value = '';
    };

    return (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-white rounded-2xl shadow-xl max-w-lg w-full p-6" onClick={e => e.stopPropagation()}>
                <div className="flex items-center justify-between mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Slike — {animalName}</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
                </div>

                {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}

                {animal && (
                    <>
                        {animal.images && animal.images.length > 0 ? (
                            <div className="grid grid-cols-3 gap-3 mb-4">
                                {animal.images.map(img => (
                                    <div key={img} className="relative group">
                                        <img src={`/${img}`} alt="" className="w-full aspect-square object-cover rounded-lg" />
                                        <button
                                            onClick={() => deleteImg.mutate(getFileName(img))}
                                            disabled={deleteImg.isPending}
                                            className="absolute top-1 right-1 bg-red-500 text-white w-6 h-6 rounded-full text-sm
                                                opacity-0 group-hover:opacity-100 transition-opacity disabled:opacity-50"
                                        >
                                            ×
                                        </button>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <p className="text-gray-500 text-sm mb-4">Nema slika.</p>
                        )}

                        {(animal.images?.length ?? 0) < 5 ? (
                            <>
                                <input
                                    ref={fileInput}
                                    type="file"
                                    accept=".jpg,.jpeg,.png,.webp,.jfif"
                                    onChange={handleFile}
                                    className="hidden"
                                />
                                <button
                                    onClick={() => fileInput.current?.click()}
                                    disabled={upload.isPending}
                                    className="w-full border-2 border-dashed border-gray-300 rounded-lg py-3 text-sm
                                        text-gray-500 hover:border-orange-400 hover:text-orange-600 transition-colors disabled:opacity-50"
                                >
                                    {upload.isPending ? 'Otpremanje...' : '+ Dodaj sliku'}
                                </button>
                            </>
                        ) : (
                            <p className="text-xs text-gray-400 text-center">Maksimalan broj slika (5) dostignut.</p>
                        )}

                        {uploadError && <p className="text-sm text-red-500 mt-2">{uploadError}</p>}
                    </>
                )}
            </div>
        </div>
    );
}