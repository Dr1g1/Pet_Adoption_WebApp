import { useState, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { shelterApi } from '../api/shelter.api';

export function SheltersPage() {
    const { data: shelters, isLoading, error } = useQuery({
        queryKey: ['shelters'],
        queryFn: () => shelterApi.getAll()
    });

    const [name, setName] = useState('');
    const [location, setLocation] = useState('');

    const filtered = useMemo(() => {
        if (!shelters) return [];
        return shelters.filter(s => {
            if (name && !s.name.toLowerCase().includes(name.toLowerCase())) return false;
            if (location && !s.address?.toLowerCase().includes(location.toLowerCase())) return false;
            return true;
        });
    }, [shelters, name, location]);

    if (isLoading) return <div className="max-w-5xl mx-auto p-8 text-gray-500">Učitavanje...</div>;
    if (error) return <div className="max-w-5xl mx-auto p-8 text-red-500">Greška pri učitavanju azila.</div>;

    const inputCls = "border border-gray-300 rounded-lg px-3 py-2 text-sm focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none";

    return (
        <div className="max-w-5xl mx-auto p-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Azili</h1>

            <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-5 mb-6">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    <input
                        placeholder="Pretraži po imenu azila..."
                        className={inputCls}
                        value={name}
                        onChange={e => setName(e.target.value)}
                    />
                    <input
                        placeholder="Pretraži po lokaciji..."
                        className={inputCls}
                        value={location}
                        onChange={e => setLocation(e.target.value)}
                    />
                </div>
                <p className="text-xs text-gray-400 mt-3">
                    Prikazano {filtered.length} od {shelters?.length ?? 0} azila
                </p>
            </div>

            {filtered.length === 0 && (
                <p className="text-gray-500">Nema azila koji odgovaraju pretrazi.</p>
            )}

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {filtered.map(s => (
                    <Link
                        key={s.id}
                        to={`/shelters/${s.id}`}
                        className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 hover:shadow-md transition-shadow"
                    >
                        <h3 className="font-semibold text-gray-900">{s.name}</h3>
                        <p className="text-sm text-gray-500 mt-1">{s.address}</p>
                        <div className="flex gap-4 mt-3 text-xs text-gray-400">
                            <span>Kapacitet: {s.capacity}</span>
                            {s.phone && <span>{s.phone}</span>}
                        </div>
                    </Link>
                ))}
            </div>
        </div>
    );
}