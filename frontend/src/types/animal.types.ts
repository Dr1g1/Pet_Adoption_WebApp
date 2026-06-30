
// Enum vrednosti — iste kao na backendu
export type Gender = 'Female' | 'Male';
export type Size = 'Small' | 'Medium' | 'Large';
export type AnimalBoolean = 'Unknown' | 'Yes' | 'No';

// Odgovara AnimalResponseDto.cs — koristi se u listi zivotinja
export interface AnimalResponseDto {
    id: string;
    breed: string;
    name: string;
    gender: Gender;
    age?: number;
    isVaccinated: AnimalBoolean;
    isSterilized: AnimalBoolean;
    primaryImgUrl?: string;
}

// Odgovara AnimalDetailResponseDto.cs — koristi se na stranici detalja
export interface AnimalDetailResponseDto {
    id: string;
    name: string;
    species: string;
    breed: string;
    age?: number;
    gender: Gender;
    size: Size;
    isVaccinated: AnimalBoolean;
    isSterilized: AnimalBoolean;
    description?: string;
    arrivedAt: string;        // DateTime → string
    images?: string[];
}

// Odgovara AnimalCreateDto.cs 
export interface AnimalCreateDto {
    name: string;
    species: string;
    breed: string;
    age?: number;
    gender: Gender;
    size: Size;
    isVaccinated: AnimalBoolean;
    isSterilized: AnimalBoolean;
    isGoodWithKids: AnimalBoolean;
    isGoodWithPets: AnimalBoolean;
    description?: string;
    arrivedAt: string;
    shelterId: string;
    caretakerId: string;
    relatedAnimalId?: string;
}

