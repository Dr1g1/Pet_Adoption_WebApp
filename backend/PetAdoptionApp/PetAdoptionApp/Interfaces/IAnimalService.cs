using PetAdoptionApp.DTOs.Animal;
namespace PetAdoptionApp.Interfaces
{
    public interface IAnimalService
    {
        Task<AnimalDetailResponseDto> CreateAnimalAsync(AnimalCreateDto dto);
        Task<bool> DeleteAnimalAsync(string animalId);
        Task<IEnumerable<AnimalResponseDto>> ReturnAllAnimals(string shelterId); //sve zivotinje u azilu (koje nisu usvojene).
        Task<AnimalResponseDto> ReturnAnimalId(string animalId);
        Task<bool> AddCaretakerAsync(string animalId, AnimalAddCaretakerDto dto);
        Task<bool> UpdateAnimalImages(string animalId, AnimalUpdateImagesDto dto);
        Task<bool> MarkAsAdoptedAsync(string animalId, AnimalAdoptedDto dto);
    }
}
