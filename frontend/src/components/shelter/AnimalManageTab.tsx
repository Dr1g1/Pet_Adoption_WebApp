import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuthStore } from '../../store/authStore';
import { animalApi, type AnimalCreatePayload } from '../../api/animal.api';
import { AnimalImagesView } from './AnimalImagesView';
import { AnimalEditedView } from './AnimalEditedView';
import { AnimalMedicalView } from './AnimalMedicalView';

export function AnimalsManageTab({ shelterId }: { shelterId: string }) {
    const queryClient = useQueryClient();
    const userId = useAuthStore(s => s.user?.id);
    const [showForm, setShowForm] = useState(false);
    const [imagesFor, setImagesFor] = useState<{ id: string; name: string } | null>(null);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [medicalFor, setMedicalFor] = useState<{ id: string; name: string } | null>(null);

    const { data: animals, isLoading } = useQuery({
        queryKey: ['animals', 'shelter', shelterId],
        queryFn: () => animalApi.getByShelter(shelterId)
    });

    const deleteAnimal = useMutation({
        mutationFn: (id: string) => animalApi.delete(id),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['animals', 'shelter', shelterId] })
    });

    const createAnimal = useMutation({
        mutationFn: (payload: AnimalCreatePayload) => animalApi.create(payload),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['animals', 'shelter', shelterId] });
            setShowForm(false);
        }
    });

    return (
        <div>
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-lg font-semibold text-gray-900">Životinje u azilu</h2>
                <button
                    onClick={() => setShowForm(s => !s)}
                    className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors"
                >
                    {showForm ? 'Otkaži' : '+ Dodaj životinju'}
                </button>
            </div>

            {showForm && (
                <AnimalForm
                    onSubmit={(data) => createAnimal.mutate({
                        ...data,
                        shelterId,
                        caretakerId: userId!,
                        arrivedAt: new Date().toISOString()
                    })}
                    isPending={createAnimal.isPending}
                    isError={createAnimal.isError}
                />
            )}

            {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
            {!isLoading && animals?.length === 0 && (
                <p className="text-gray-500 text-sm">Nema životinja.</p>
            )}

            <div className="space-y-2 mt-4">
                {animals?.map(a => (
                    <div key={a.id} className="flex items-center justify-between border border-gray-100 rounded-lg p-3">
                        <div className="flex items-center gap-3">
                            <span className="text-xl"></span>
                            <div>
                                <p className="text-sm font-medium text-gray-800">{a.name}</p>
                                <p className="text-xs text-gray-400">{a.breed}</p>
                            </div>
                        </div>
                        <div className="flex items-center gap-3">
                            <button
                                onClick={() => setEditingId(a.id)}
                                className="text-xs text-blue-600 hover:text-blue-700"
                            >
                                Izmeni
                            </button>
                            <button
                                onClick={() => setImagesFor({ id: a.id, name: a.name })}
                                className="text-xs text-orange-600 hover:text-orange-700"
                            >
                                Slike
                            </button>
                            <button
                                onClick={() => setMedicalFor({ id: a.id, name: a.name })}
                                className="text-xs text-green-600 hover:text-green-700"
                            >
                                Kartoni
                            </button>
                            <button
                                onClick={() => {
                                    if (confirm(`Obrisati ${a.name}?`)) deleteAnimal.mutate(a.id);
                                }}
                                disabled={deleteAnimal.isPending}
                                className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
                            >
                                Obriši
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {imagesFor && (
                <AnimalImagesView
                    animalId={imagesFor.id}
                    animalName={imagesFor.name}
                    onClose={() => setImagesFor(null)}
                />
            )}

            {editingId && (
                <AnimalEditedView
                    animalId={editingId}
                    shelterId={shelterId}
                    onClose={() => setEditingId(null)}
                />
            )}

            {medicalFor && (
                <AnimalMedicalView
                    animalId={medicalFor.id}
                    animalName={medicalFor.name}
                    canEdit={true}
                    onClose={() => setMedicalFor(null)}
                />
            )}
        </div>
    );
}

function AnimalForm({
    onSubmit, isPending, isError
}: {
    onSubmit: (data: Omit<AnimalCreatePayload, 'shelterId' | 'caretakerId' | 'arrivedAt'>) => void;
    isPending: boolean;
    isError: boolean;
}) {
    const [form, setForm] = useState({
        name: '', species: '', breed: '', age: undefined as number | undefined,
        gender: 'Female' as 'Female' | 'Male',
        size: 'Medium' as 'Small' | 'Medium' | 'Large',
        isVaccinated: 'Unknown' as 'Unknown' | 'Yes' | 'No',
        isSterilized: 'Unknown' as 'Unknown' | 'Yes' | 'No',
        isGoodWithKids: 'Unknown' as 'Unknown' | 'Yes' | 'No',
        isGoodWithPets: 'Unknown' as 'Unknown' | 'Yes' | 'No',
        description: ''
    });

    const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none";

    return (
        <div className="bg-gray-50 border border-gray-200 rounded-xl p-5 mb-4 space-y-3">
            <div className="grid grid-cols-2 gap-3">
                <input placeholder="Ime *" className={inputCls}
                    value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} />
                <input placeholder="Vrsta (pas, mačka) *" className={inputCls}
                    value={form.species} onChange={e => setForm(f => ({ ...f, species: e.target.value }))} />
                <input placeholder="Rasa" className={inputCls}
                    value={form.breed} onChange={e => setForm(f => ({ ...f, breed: e.target.value }))} />
                <input type="number" placeholder="Starost (god.)" className={inputCls}
                    value={form.age ?? ''} onChange={e => setForm(f => ({ ...f, age: e.target.value ? +e.target.value : undefined }))} />

                <select className={inputCls} value={form.gender}
                    onChange={e => setForm(f => ({ ...f, gender: e.target.value as 'Female' | 'Male' }))}>
                    <option value="Female">Ženka</option>
                    <option value="Male">Mužjak</option>
                </select>
                <select className={inputCls} value={form.size}
                    onChange={e => setForm(f => ({ ...f, size: e.target.value as 'Small' | 'Medium' | 'Large' }))}>
                    <option value="Small">Mala</option>
                    <option value="Medium">Srednja</option>
                    <option value="Large">Velika</option>
                </select>

                <BoolSelect label="Vakcinisan" value={form.isVaccinated}
                    onChange={v => setForm(f => ({ ...f, isVaccinated: v }))} cls={inputCls} />
                <BoolSelect label="Sterilisan" value={form.isSterilized}
                    onChange={v => setForm(f => ({ ...f, isSterilized: v }))} cls={inputCls} />
                <BoolSelect label="Dobar s decom" value={form.isGoodWithKids}
                    onChange={v => setForm(f => ({ ...f, isGoodWithKids: v }))} cls={inputCls} />
                <BoolSelect label="Dobar s ljubimcima" value={form.isGoodWithPets}
                    onChange={v => setForm(f => ({ ...f, isGoodWithPets: v }))} cls={inputCls} />
            </div>
            <textarea placeholder="Opis" rows={2} className={inputCls}
                value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />

            {isError && <p className="text-sm text-red-500">Greška pri dodavanju. Proveri polja.</p>}

            <button
                onClick={() => onSubmit(form)}
                disabled={isPending || !form.name || !form.species}
                className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-5 py-2 rounded-lg transition-colors disabled:opacity-50"
            >
                {isPending ? 'Dodavanje...' : 'Sačuvaj životinju'}
            </button>
        </div>
    );
}

function BoolSelect({ label, value, onChange, cls }: {
    label: string;
    value: 'Unknown' | 'Yes' | 'No';
    onChange: (v: 'Unknown' | 'Yes' | 'No') => void;
    cls: string;
}) {
    return (
        <select className={cls} value={value} onChange={e => onChange(e.target.value as 'Unknown' | 'Yes' | 'No')}>
            <option value="Unknown">{label}: Nepoznato</option>
            <option value="Yes">{label}: Da</option>
            <option value="No">{label}: Ne</option>
        </select>
    );
}