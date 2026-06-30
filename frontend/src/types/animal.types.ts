

export type Gender = 'Female' | 'Male';
export type Size = 'Small' | 'Medium' | 'Large';
export type AnimalBoolean = 'Unknown' | 'Yes' | 'No';

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
    arrivedAt: string;
    images?: string[];
}

export interface AnimalResponseDto {
    id: string;
    breed: string;
    name: string;
    species: string;
    gender: Gender;
    size: Size;
    age?: number;
    isVaccinated: AnimalBoolean;
    isSterilized: AnimalBoolean;
    isGoodWithKids: AnimalBoolean;
    isGoodWithPets: AnimalBoolean;
    primaryImgUrl?: string;
    isAdopted: boolean;
}

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

