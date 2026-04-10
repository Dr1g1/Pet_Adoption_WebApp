using PetAdoptionApp.DTOs;
namespace PetAdoptionApp.Interfaces
{
    public interface IAnimalService
    {
        Task<DTOs.Animal.AnimalResponseDto> CreateAnimal(DTOs.Animal.AnimalCreateDto dto);
        Task<bool> DeleteAnimal(string animalId);

    }
}
