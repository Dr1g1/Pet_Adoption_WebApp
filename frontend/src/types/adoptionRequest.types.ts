export type RequestStatus = 'Pending' | 'Approved' | 'Rejected';

// Odgovara AdoptionRequestUserCreateDto.cs — korisnik salje zahtev
export interface AdoptionRequestUserCreateDto {
    animalId: string;
    message: string;
}

// Odgovara AdoptionRequestActionDto.cs — volonter odgovara na zahtev
export interface AdoptionRequestActionDto {
    requestId: string;
    responseMessage: string;
}

// Odgovara AdoptionRequestReturnDto.cs — za prikaz u listi (volonter vidi)
export interface AdoptionRequestReturnDto {
    requestId: string;
    userId: string;
    userName: string;
    animalId: string;
    animalName: string;
    status: RequestStatus;
    createdAt: string;
}

// Odgovara AdoptionRequestUserResponseDto.cs — korisnik vidi svoje zahteve
export interface AdoptionRequestUserResponseDto {
    requestId: string;
    status: RequestStatus;
    createdAt: string;
    animalId: string;
    shelterId: string;
}