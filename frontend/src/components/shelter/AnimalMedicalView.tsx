import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { medicalRecordApi } from '../../api/medicalRecord.api';
import type { MedicalRecordCreateDto } from '../../types/medicalRecord.types';

export function AnimalMedicalView({ animalId, animalName, canEdit, onClose }: {
    animalId: string;
    animalName: string;
    canEdit: boolean;
    onClose: () => void;
}) {
    const queryClient = useQueryClient();
    const [showForm, setShowForm] = useState(false);

    const { data: records, isLoading } = useQuery({
        queryKey: ['medicalRecords', animalId],
        queryFn: () => medicalRecordApi.getForAnimal(animalId)
    });

    const createRecord = useMutation({
        mutationFn: (dto: MedicalRecordCreateDto) => medicalRecordApi.create(animalId, dto),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['medicalRecords', animalId] });
            setShowForm(false);
        }
    });

    const deleteRecord = useMutation({
        mutationFn: (recordId: string) => medicalRecordApi.delete(recordId),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['medicalRecords', animalId] })
    });

    return (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center p-4 z-50" onClick={onClose}>
            <div className="bg-white rounded-2xl shadow-xl max-w-lg w-full p-6 max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
                <div className="flex items-center justify-between mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Medicinski kartoni — {animalName}</h3>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
                </div>

                {canEdit && !showForm && (
                    <button
                        onClick={() => setShowForm(true)}
                        className="mb-4 bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors"
                    >
                        + Dodaj karton
                    </button>
                )}

                {showForm && (
                    <MedicalForm
                        onSubmit={(dto) => createRecord.mutate(dto)}
                        onCancel={() => setShowForm(false)}
                        isPending={createRecord.isPending}
                        isError={createRecord.isError}
                    />
                )}

                {isLoading && <p className="text-gray-500 text-sm">Učitavanje...</p>}
                {!isLoading && records?.length === 0 && (
                    <p className="text-gray-500 text-sm">Nema medicinskih kartona.</p>
                )}

                <div className="space-y-3">
                    {records?.map(rec => (
                        <div key={rec.id} className="border border-gray-100 rounded-xl p-4">
                            <div className="flex items-start justify-between">
                                <div>
                                    <p className="text-sm font-medium text-gray-800">{rec.description}</p>
                                    <p className="text-xs text-gray-400 mt-1">
                                        {new Date(rec.date).toLocaleDateString('sr-RS')} · {rec.vetName}
                                    </p>
                                </div>
                                {canEdit && (
                                    <button
                                        onClick={() => {
                                            if (confirm('Obrisati karton?')) deleteRecord.mutate(rec.id);
                                        }}
                                        disabled={deleteRecord.isPending}
                                        className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
                                    >
                                        Obriši
                                    </button>
                                )}
                            </div>

                            <div className="grid grid-cols-2 gap-2 mt-3 text-xs">
                                <div>
                                    <span className="text-gray-400">Klinika: </span>
                                    <span className="text-gray-700">{rec.clinicPhone}</span>
                                </div>
                                <div>
                                    <span className="text-gray-400">Sledeća kontrola: </span>
                                    <span className="text-gray-700">{new Date(rec.nextDueDate).toLocaleDateString('sr-RS')}</span>
                                </div>
                            </div>

                            {rec.vaccines && rec.vaccines.length > 0 && (
                                <div className="mt-2">
                                    <div className="flex flex-wrap gap-1">
                                        {rec.vaccines.map((v, i) => (
                                            <span key={i} className="text-xs bg-green-50 text-green-700 px-2 py-0.5 rounded-full">{v}</span>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

function MedicalForm({ onSubmit, onCancel, isPending, isError }: {
    onSubmit: (dto: MedicalRecordCreateDto) => void;
    onCancel: () => void;
    isPending: boolean;
    isError: boolean;
}) {
    const today = new Date().toISOString().split('T')[0];
    const [form, setForm] = useState({
        description: '',
        date: today,
        clinicPhone: '',
        vetName: '',
        nextDueDate: today,
        vaccinesText: ''
    });

    const inputCls = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none";

    const submit = () => {
        onSubmit({
            description: form.description,
            date: form.date,
            clinicPhone: form.clinicPhone,
            vetName: form.vetName,
            nextDueDate: form.nextDueDate,
            vaccines: form.vaccinesText
                .split(',')
                .map(v => v.trim())
                .filter(v => v.length > 0)
        });
    };

    return (
        <div className="bg-gray-50 border border-gray-200 rounded-xl p-4 mb-4 space-y-3">
            <div>
                <label className="block text-xs text-gray-400 mb-1">Opis *</label>
                <textarea rows={2} className={inputCls} value={form.description}
                    onChange={e => setForm(f => ({ ...f, description: e.target.value }))} />
            </div>
            <div className="grid grid-cols-2 gap-3">
                <div>
                    <label className="block text-xs text-gray-400 mb-1">Datum</label>
                    <input type="date" className={inputCls} value={form.date}
                        onChange={e => setForm(f => ({ ...f, date: e.target.value }))} />
                </div>
                <div>
                    <label className="block text-xs text-gray-400 mb-1">Sledeća kontrola</label>
                    <input type="date" className={inputCls} value={form.nextDueDate}
                        onChange={e => setForm(f => ({ ...f, nextDueDate: e.target.value }))} />
                </div>
                <div>
                    <label className="block text-xs text-gray-400 mb-1">Veterinar</label>
                    <input className={inputCls} value={form.vetName}
                        onChange={e => setForm(f => ({ ...f, vetName: e.target.value }))} />
                </div>
                <div>
                    <label className="block text-xs text-gray-400 mb-1">Telefon klinike</label>
                    <input className={inputCls} value={form.clinicPhone}
                        onChange={e => setForm(f => ({ ...f, clinicPhone: e.target.value }))} />
                </div>
            </div>
            <div>
                <label className="block text-xs text-gray-400 mb-1">Vakcine (odvojene zarezom)</label>
                <input className={inputCls} placeholder="npr. Besnilo, Štenećak"
                    value={form.vaccinesText}
                    onChange={e => setForm(f => ({ ...f, vaccinesText: e.target.value }))} />
            </div>

            {isError && <p className="text-sm text-red-500">Greška pri čuvanju.</p>}

            <div className="flex gap-2">
                <button onClick={submit} disabled={isPending || !form.description}
                    className="bg-orange-500 hover:bg-orange-600 text-white text-sm font-medium px-4 py-2 rounded-lg transition-colors disabled:opacity-50">
                    {isPending ? 'Čuvanje...' : 'Sačuvaj'}
                </button>
                <button onClick={onCancel}
                    className="text-gray-600 hover:bg-gray-100 text-sm px-4 py-2 rounded-lg transition-colors">
                    Otkaži
                </button>
            </div>
        </div>
    );
}