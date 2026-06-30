using PetAdoptionApp.Common;

namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalResponseDto
    {
        public string id { get; set; }
        public string breed { get; set; }
        public string name { get; set; }
        public string species { get; set; }
        public Enums.Gender gender { get; set; }
        public Enums.Size size { get; set; }
        public int? age { get; set; }
        public Enums.AnimalBoolean isVaccinated { get; set; }
        public Enums.AnimalBoolean isSterilized { get; set; }
        public Enums.AnimalBoolean isGoodWithKids { get; set; }  
        public Enums.AnimalBoolean isGoodWithPets { get; set; }  
        public string? primaryImgUrl { get; set; }
        public bool isAdopted { get; set; }
    }
}