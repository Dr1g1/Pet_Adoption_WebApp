using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalCreateDto
    {
        public string name { get; set; }
        public string species { get; set; }
        public string breed { get; set; }
        public int? age { get; set; }
        public Enums.Gender gender { get; set; }
        public Enums.Size size { get; set; }
        public Boolean isVaccinated { get; set; }
        public Boolean isSterilized { get; set; }
        public Boolean isGoodWithKids { get; set; }
        public Boolean isGoodWithPets { get; set; }
        public string? description { get; set; }
        public DateTime arrivedAt { get; set; }
        public List<string>? images { get; set; }
        //ne prosledjujemo ceo shelter ili animal objekat, samo id.
        public string shelterId { get; set; }
        public string caretakerId { get; set; } //moramo kad pravimo zivotinju odmah da dodamo caretaker-a.
        public string? relatedAnimalId { get; set; }
    }
}
