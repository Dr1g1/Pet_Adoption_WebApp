// Interfaces/IAnimalService.cs
using PetAdoptionApp.DTOs.Animal;

namespace PetAdoptionApp.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDetailResponseDto> CreateAnimalAsync(AnimalCreateDto dto);
        Task<bool> DeleteAnimalAsync(string animalId);
        Task<IEnumerable<AnimalResponseDto>> GetAllAnimalsAsync(string shelterId);
        Task<AnimalResponseDto?> GetAnimalByIdAsync(string animalId);
        Task<bool> AddCaretakerAsync(string animalId, string caretakerId);   // ← string, ne DTO
        Task<bool> MarkAsAdoptedAsync(string animalId, string userId);        // ← string, ne DTO
        Task<bool> AddRelativeAsync(string animalId, string relativeId);      // ← string, ne DTO
        Task<string?> AddImageAsync(string animalId, IFormFile file);
        Task<bool> RemoveImageAsync(string animalId, string fileName);
    }
}