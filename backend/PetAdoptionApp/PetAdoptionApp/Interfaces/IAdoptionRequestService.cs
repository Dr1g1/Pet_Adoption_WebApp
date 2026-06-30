using PetAdoptionApp.DTOs.AdoptionRequest;
namespace PetAdoptionApp.Interfaces
{
    public interface IAdoptionRequestService
    {       
        Task<AdoptionRequestUserResponseDto> CreateAdoptionRequestAsync(string userId, AdoptionRequestUserCreateDto dto);
        Task<bool> RejectRequestAsync(AdoptionRequestActionDto dto, string shelterId);
        Task<bool> ApprovedRequestAsync(AdoptionRequestActionDto dto, string shelterId);
        Task<IEnumerable<AdoptionRequestReturnDto>> GetPendingRequests(string shelterId); 
        Task<IEnumerable<AdoptionRequestReturnDto>> GetRequestsForAnimal(string animalId);
        Task<bool> DeleteRequestAsync(string requestId, string userId);
        Task<IEnumerable<AdoptionRequestReturnDto>> GetRequestsForUser(string userId);
    }
}
