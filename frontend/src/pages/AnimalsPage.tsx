import { useState, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { animalApi } from '../api/animal.api';
import type { AnimalBoolean } from '../types/animal.types';

const boolLabel: Record<AnimalBoolean, string> = {
    Yes: 'Da', No: 'Ne', Unknown: 'Nepoznato'
};

export function AnimalsPage() {
    const { data: animals, isLoading, error } = useQuery({
        queryKey: ['animals', 'available'],
        queryFn: () => animalApi.getAllAvailable()
    });

    const [name, setName] = useState('');
    const [species, setSpecies] = useState('');
    const [gender, setGender] = useState('');
    const [size, setSize] = useState('');
    const [vaccinated, setVaccinated] = useState('');
    const [goodWithKids, setGoodWithKids] = useState('');
    const [goodWithPets, setGoodWithPets] = useState('');

    const filtered = useMemo(() => {
        if (!animals) return [];
        return animals.filter(a => {
            if (name && !a.name.toLowerCase().includes(name.toLowerCase())) return false;
            if (species && !a.species?.toLowerCase().includes(species.toLowerCase())) return false;
            if (gender && a.gender !== gender) return false;
            if (size && a.size !== size) return false;
            if (vaccinated && a.isVaccinated !== vaccinated) return false;
            if (goodWithKids && a.isGoodWithKids !== goodWithKids) return false;
            if (goodWithPets && a.isGoodWithPets !== goodWithPets) return false;
            return true;
        });
    }, [animals, name, species, gender, size, vaccinated, goodWithKids, goodWithPets]);

    const resetFilters = () => {
        setName(''); setSpecies(''); setGender(''); setSize('');
        setVaccinated(''); setGoodWithKids(''); setGoodWithPets('');
    };

    if (isLoading) return <div className="max-w-5xl mx-auto p-8 text-gray-500">Učitavanje...</div>;
    if (error) return <div className="max-w-5xl mx-auto p-8 text-red-500">Greška pri učitavanju životinja.</div>;

    const selectCls = "border border-gray-300 rounded-lg px-3 py-2 text-sm focus:border-orange-400 focus:ring-2 focus:ring-orange-100 outline-none";

    return (
        <div className="max-w-5xl mx-auto p-8">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Životinje za udomljavanje</h1>

            {/* Filteri */}
            <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-5 mb-6">
                <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
                    <input
                        placeholder="Pretraži po imenu..."
                        className={selectCls}
                        value={name}
                        onChange={e => setName(e.target.value)}
                    />
                    <input
                        placeholder="Vrsta (pas, mačka)..."
                        className={selectCls}
                        value={species}
                        onChange={e => setSpecies(e.target.value)}
                    />
                    <select className={selectCls} value={gender} onChange={e => setGender(e.target.value)}>
                        <option value="">Pol (svi)</option>
                        <option value="Female">Ženka</option>
                        <option value="Male">Mužjak</option>
                    </select>
                    <select className={selectCls} value={size} onChange={e => setSize(e.target.value)}>
                        <option value="">Veličina (sve)</option>
                        <option value="Small">Mala</option>
                        <option value="Medium">Srednja</option>
                        <option value="Large">Velika</option>
                    </select>
                    <select className={selectCls} value={vaccinated} onChange={e => setVaccinated(e.target.value)}>
                        <option value="">Vakcinisan (svi)</option>
                        <option value="Yes">Vakcinisan: Da</option>
                        <option value="No">Vakcinisan: Ne</option>
                        <option value="Unkown">Vakcinisan: Nepoznato</option>
                    </select>
                    <select className={selectCls} value={goodWithKids} onChange={e => setGoodWithKids(e.target.value)}>
                        <option value="">Dobar s decom (svi)</option>
                        <option value="Yes">S decom: Da</option>
                        <option value="No">S decom: Ne</option>
                        <option value="Unkown">S decom: Nepoznato</option>
                    </select>
                    <select className={selectCls} value={goodWithPets} onChange={e => setGoodWithPets(e.target.value)}>
                        <option value="">Dobar s ljubimcima (svi)</option>
                        <option value="Yes">S ljubimcima: Da</option>
                        <option value="No">S ljubimcima: Ne</option>
                        <option value="Unkown">S ljubimcima: Nepoznato</option>
                    </select>
                    <button
                        onClick={resetFilters}
                        className="text-sm text-gray-600 hover:bg-gray-100 rounded-lg px-3 py-2 transition-colors"
                    >
                        Poništi filtere
                    </button>
                </div>
                <p className="text-xs text-gray-400 mt-3">
                    Prikazano {filtered.length} od {animals?.length ?? 0} životinja
                </p>
            </div>

            {filtered.length === 0 && (
                <p className="text-gray-500">Nema životinja koje odgovaraju filterima.</p>
            )}

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
                {filtered.map(a => (
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
                            <p className="text-sm text-gray-500">{a.species} · {a.breed}</p>
                            <div className="flex gap-2 mt-3 text-xs flex-wrap">
                                <span className="bg-gray-100 px-2 py-1 rounded-full">
                                    {a.gender === 'Male' ? 'Mužjak' : 'Ženka'}
                                </span>
                                {a.age != null && (
                                    <span className="bg-gray-100 px-2 py-1 rounded-full">{a.age} god.</span>
                                )}
                                <span className="bg-gray-100 px-2 py-1 rounded-full">
                                    Vakcinisan: {boolLabel[a.isVaccinated]}
                                </span>
                            </div>
                        </div>
                    </Link>
                ))}
            </div>
        </div>
    );
}