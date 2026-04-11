using PetAdoptionApp.DTOs.AdoptionRequest;
namespace PetAdoptionApp.Interfaces
{
    public interface IAdoptionRequestService
    {       
        Task<AdoptionRequestUserResponseDto> CreateAdoptionRequestAsync(string userId, AdoptionRequestUserCreateDto dto);
        //Task<bool> AdoptionRequestActionAsync(AdoptionRequestActionDto dto);
        //treba da se dodaju funkcije za odbijanje i prihvatanje adoption requesta
        //ove funkcije su vidljive samo VOLONTERIMA koji rade u azilu.
        Task<bool> RejectRequestAsync(AdoptionRequestActionDto dto, string shelterId);
        Task<bool> ApprovedRequestAsync(AdoptionRequestActionDto dto, string shelterId);
        Task<IEnumerable<AdoptionRequestReturnDto>> GetPendingRequests(string shelterId); //vratiti samo pending zaheteve
        Task<IEnumerable<AdoptionRequestReturnDto>> GetRequestsForAnimal(string animalId);
        Task<bool> DeleteRequestAsync(string requestId, string userId);
    }
}
