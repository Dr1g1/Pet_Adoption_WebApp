import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { animalApi, type AnimalUpdatePayload } from '../../api/animal.api';

export function AnimalEditedView({ animalId, shelterId, onClose }: {
    animalId: string;
    shelterId: string;
    onClose: () => void;
}) {
    const queryClient = useQueryClient();
    const [form, setForm] = useState<AnimalUpdatePayload>({});
    const [loaded, setLoaded] = useState(false);

    const { data: animal, isLoading } = useQuery({
        queryKey: ['animal', animalId],
        queryFn: () => animalApi.getById(animalId)
    });

    if (animal && !loaded) {
        setForm({
            name: animal.name,
            species: animal.species,
            breed: animal.breed,
            age: animal.age,
            gender: animal.gender,
            size: animal.size,
            isVaccinated: animal.isVaccinated,
            isSterilized: animal.isSterilized,
            description: animal.description ?? ''
        });
        setLoaded(true);
    }

    const update = useMutation({
        mutationFn: () => animalApi.update(animalId, form),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['animal', animalId] });
            queryClient.invalidateQueries({ queryKey: ['animals', 'shelter', shelterId] });
            onClose();
        }
    });

    const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none";

    return (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-white rounded-2xl shadow-xl max-w-lg w-full p-6 max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
                <div className="flex items-center justify-between mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Izmena životinje</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
                </div>

                {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}

                {animal && (
                    <div className="space-y-3">
                        <div className="grid grid-cols-2 gap-3">
                            <Field label="Ime">
                                <input className={inputCls} value={form.name ?? ''}
                                    onChange={e => setForm(f => ({ ...f, name: e.target.value }))} />
                            </Field>
                            <Field label="Vrsta">
                                <input className={inputCls} value={form.species ?? ''}
                                    onChange={e => setForm(f => ({ ...f, species: e.target.value }))} />
                            </Field>
                            <Field label="Rasa">
                                <input className={inputCls} value={form.breed ?? ''}
                                    onChange={e => setForm(f => ({ ...f, breed: e.target.value }))} />
                            </Field>
                            <Field label="Starost (god.)">
                                <input type="number" className={inputCls} value={form.age ?? ''}
                                    onChange={e => setForm(f => ({ ...f, age: e.target.value ? +e.target.value : undefined }))} />
                            </Field>
                            <Field label="Pol">
                                <select className={inputCls} value={form.gender ?? 'Female'}
                                    onChange={e => setForm(f => ({ ...f, gender: e.target.value as 'Female' | 'Male' }))}>
                                    <option value="Female">Ženka</option>
                                    <option value="Male">Mužjak</option>
                                </select>
                            </Field>
                            <Field label="Veličina">
                                <select className={inputCls} value={form.size ?? 'Medium'}
                                    onChange={e => setForm(f => ({ ...f, size: e.target.value as 'Small' | 'Medium' | 'Large' }))}>
                                    <option value="Small">Mala</option>
                                    <option value="Medium">Srednja</option>
                                    <option value="Large">Velika</option>
                                </select>
                            </Field>
                            <BoolField label="Vakcinisan" value={form.isVaccinated ?? 'Unknown'}
                                onChange={v => setForm(f => ({ ...f, isVaccinated: v }))} cls={inputCls} />
                            <BoolField label="Sterilisan" value={form.isSterilized ?? 'Unknown'}
                                onChange={v => setForm(f => ({ ...f, isSterilized: v }))} cls={inputCls} />
                        </div>

                        <Field label="Opis">
                            <textarea rows={3} className={inputCls} value={form.description ?? ''}
                                onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
                        </Field>

                        {update.isError && <p className="text-sm text-red-500">Greška pri čuvanju.</p>}

                        <div className="flex gap-2 pt-2">
                            <button
                                onClick={() => update.mutate()}
                                disabled={update.isPending}
                                className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-5 py-2 rounded-lg transition-colors disabled:opacity-50"
                            >
                                {update.isPending ? 'Čuvanje...' : 'Sačuvaj'}
                            </button>
                            <button onClick={onClose}
                                className="text-gray-600 hover:bg-gray-100 text-sm px-5 py-2 rounded-lg transition-colors">
                                Otkaži
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
    return (
        <div>
            <label className="block text-xs text-gray-400 mb-1">{label}</label>
            {children}
        </div>
    );
}

function BoolField({ label, value, onChange, cls }: {
    label: string;
    value: 'Unknown' | 'Yes' | 'No';
    onChange: (v: 'Unknown' | 'Yes' | 'No') => void;
    cls: string;
}) {
    return (
        <div>
            <label className="block text-xs text-gray-400 mb-1">{label}</label>
            <select className={cls} value={value} onChange={e => onChange(e.target.value as 'Unknown' | 'Yes' | 'No')}>
                <option value="Unknown">Nepoznato</option>
                <option value="Yes">Da</option>
                <option value="No">Ne</option>
            </select>
        </div>
    );
}