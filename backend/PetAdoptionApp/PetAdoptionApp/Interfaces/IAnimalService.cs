using PetAdoptionApp.DTOs;
namespace PetAdoptionApp.Interfaces
{
    public interface IAnimalService
    {
        Task<DTOs.Animal.AnimalDetailResponseDto> CreateAnimalAsync(DTOs.Animal.AnimalCreateDto dto);
        Task<bool> DeleteAnimalAsync(string animalId);
        Task<DTOs.Animal.AnimalResponseDto> ReturnAllAnimals(string shelterId); //sve zivotinje u azilu (koje nisu usvojene).
        Task<DTOs.Animal.AnimalResponseDto> ReturnAnimalId(string animalId);
        Task<bool> AddCaretakerAsync(string animalId, DTOs.Animal.AnimalAddCaretakerDto dto);
        Task<bool> UpdateAnimalImages(string animalId, DTOs.Animal.AnimalUpdateImagesDto dto);
        Task<bool> MarkAsAdoptedAsync(string animalId, DTOs.Animal.AnimalAdoptedDto dto);
    }
}
