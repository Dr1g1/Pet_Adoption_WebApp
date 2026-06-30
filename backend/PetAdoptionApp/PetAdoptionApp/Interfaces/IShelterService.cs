

using PetAdoptionApp.DTOs.Shelter;

namespace PetAdoptionApp.Interfaces
{
    public interface IShelterService
    {
        Task<ShelterDto> CreateShelterAsync(ShelterDto createDto);
        Task<List<ShelterDto>> GetAllSheltersAsync();
        Task<ShelterDto?> GetShelterByIdAsync(string id);
        Task<ShelterDto?> UpdateShelterAsync(string id, ShelterDto updateDto);
        Task<bool> DeleteShelterAsync(string id);
    }
}
