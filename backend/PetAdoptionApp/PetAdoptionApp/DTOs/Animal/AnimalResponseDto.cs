using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalResponseDto
    {
        public string id { get; set; }
        public string breed { get; set; }
        public string name { get; set; }
        public Enums.Gender gender { get; set; }
        public int age { get; set; }
        public Boolean isVaccinated { get; set; }
        public Boolean isSterilized { get; set; }
        public string? primaryImgUrl { get; set; }
    }
}
