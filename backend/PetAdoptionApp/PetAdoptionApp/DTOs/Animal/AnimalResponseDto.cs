using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalResponseDto
    {
        public string id { get; set; }
        public string breed { get; set; }
        public string name { get; set; }
        public Enums.Gender gender { get; set; }
        public int? age { get; set; }
        public Enums.AnimalBoolean isVaccinated { get; set; }
        public Enums.AnimalBoolean isSterilized { get; set; }
        public string? primaryImgUrl { get; set; }
    }
}
