using PetAdoptionApp.DTOs;
namespace PetAdoptionApp.Interfaces
{
    public interface IAdoptionRequestService
    {
        Task<DTOs.AdoptionRequest.AdoptionRequestReturnDto> CreateAdoptionRequestAsync(DTOs.AdoptionRequest.AdoptionRequestUserCreateDto dto);
        Task<bool> AdoptionRequestActionAsync(DTOs.AdoptionRequest.AdoptionRequestActionDto dto);
        Task<IEnumerable<DTOs.AdoptionRequest.AdoptionRequestReturnDto>> GetPendingRequests(); //vratiti samo pending zaheteve
        Task<IEnumerable<DTOs.AdoptionRequest.AdoptionRequestReturnDto>> GetRequestsForAnimal(string animalId);
        Task<bool> CancelRequestAsync(string requestId, string userId);
    }
}
