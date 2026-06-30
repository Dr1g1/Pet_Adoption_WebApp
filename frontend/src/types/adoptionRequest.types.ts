export type AdoptionRequestStatus = 'Pending' | 'Approved' | 'Rejected';


export interface AdoptionRequestUserCreateDto {
    animalId: string;
    message: string;
}

export interface AdoptionRequestActionDto {
    requestId: string;
    responseMessage: string;
}

export interface AdoptionRequestReturnDto {
    requestId: string;
    userId: string;
    userName: string;
    animalId: string;
    animalName: string;
    status: AdoptionRequestStatus;
    createdAt: string;
}

export interface AdoptionRequestUserResponseDto {
    requestId: string;
    status: AdoptionRequestStatus;
    createdAt: string;
    animalId: string;
    shelterId: string;
}