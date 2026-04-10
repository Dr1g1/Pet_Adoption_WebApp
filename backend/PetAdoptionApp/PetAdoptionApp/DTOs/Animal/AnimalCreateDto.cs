using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalCreateDto
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public int? Age { get; set; }
        public Enums.Gender Gender { get; set; }
        public Enums.Size Size { get; set; }
        public Enums.AnimalBoolean IsVaccinated { get; set; }
        public Enums.AnimalBoolean IsSterilized { get; set; }
        public Enums.AnimalBoolean IsGoodWithKids { get; set; }
        public Enums.AnimalBoolean IsGoodWithPets { get; set; }
        public string? Description { get; set; }
        public DateTime ArrivedAt { get; set; }
        public string ShelterId { get; set; }
        public string CaretakerId { get; set; } 
        public string? RelatedAnimalId { get; set; }
    }
}
