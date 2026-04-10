using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalResponseDto
    {
        public string id { get; set; }
        public string breed { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public bool isVaccinated { get; set; }
        public bool isSterilized { get; set; }
        public string? primaryImgUrl { get; set; }
    }
}
