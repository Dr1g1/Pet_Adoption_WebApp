export type JoinRequestStatus = 'Pending' | 'Approved' | 'Rejected';

export interface JoinRequestReturnDto {
    requestId: string;
    volunteerId: string;
    volunteerName: string;
    shelterId: string;
    shelterName: string;
    status: JoinRequestStatus;
    createdAt: string;
    message?: string;
}