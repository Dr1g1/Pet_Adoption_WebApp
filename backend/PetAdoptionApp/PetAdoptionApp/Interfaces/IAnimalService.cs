
using PetAdoptionApp.DTOs.Animal;

namespace PetAdoptionApp.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDetailResponseDto> CreateAnimalAsync(AnimalCreateDto dto);
        Task<bool> DeleteAnimalAsync(string animalId);
        Task<IEnumerable<AnimalResponseDto>> GetAllAnimalsAsync(string shelterId);
        Task<AnimalDetailResponseDto?> GetAnimalByIdAsync(string animalId);
        Task<bool> AddCaretakerAsync(string animalId, string caretakerId);   
        Task<bool> MarkAsAdoptedAsync(string animalId, string userId);       
        Task<bool> AddRelativeAsync(string animalId, string relativeId);     
        Task<string?> AddImageAsync(string animalId, IFormFile file);
        Task<bool> RemoveImageAsync(string animalId, string fileName);
        Task<IEnumerable<AnimalResponseDto>> GetAllAvailableAnimalsAsync();
        Task<AnimalDetailResponseDto?> UpdateAnimalAsync(string animalId, AnimalUpdateDto dto);
        Task<IEnumerable<AnimalResponseDto>> GetAllByShelterIncludingAdoptedAsync(string shelterId);
        Task<bool> LikeAnimalAsync(string userId, string animalId);
        Task<bool> UnlikeAnimalAsync(string userId, string animalId);
        Task<IEnumerable<AnimalResponseDto>> GetLikedAnimalsAsync(string userId);
        Task<int> GetLikeCountAsync(string animalId);
    }
}