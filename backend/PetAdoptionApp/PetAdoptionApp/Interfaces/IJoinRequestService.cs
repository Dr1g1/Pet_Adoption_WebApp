using PetAdoptionApp.DTOs.JoinRequest;

namespace PetAdoptionApp.Interfaces
{
    public interface IJoinRequestService
    {
        Task<JoinRequestReturnDto> CreateAsync(string volunteerId, JoinRequestCreateDto dto);
        Task<IEnumerable<JoinRequestReturnDto>> GetMyRequestsAsync(string volunteerId);
        Task<IEnumerable<JoinRequestReturnDto>> GetPendingForShelterAsync(string shelterId);
        Task<bool> ApproveAsync(string requestId, string shelterId);
        Task<bool> RejectAsync(string requestId, string shelterId);
        Task<bool> CancelAsync(string requestId, string volunteerId);
    }
}