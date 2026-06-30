using PetAdoptionApp.DTOs.Volunteer;


namespace PetAdoptionApp.Interfaces
{
    public interface IVolunteerService
    {
        Task<List<VolunteerSummaryDto>> GetAllAsync();
        Task<List<VolunteerSummaryDto>> GetByShelterAsync(string shelterId);
        Task<VolunteerDto?> GetByIdAsync(string id);
        Task<VolunteerDto> CreateAsync(VolunteerCreateDto dto);
        Task<VolunteerDto?> UpdateAsync(string id, VolunteerUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> AssignToShelterAsync(string volunteerId, string shelterId);
    }
}
