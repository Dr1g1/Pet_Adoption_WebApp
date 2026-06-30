using PetAdoptionApp.DTOs.Shelter;
using PetAdoptionApp.DTOs.MedicalRecord;
using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalDetailResponseDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string species { get; set; }
        public string breed { get; set; }
        public int? age { get; set; }
        public Enums.Gender gender { get; set; }
        public Enums.Size size { get; set; }
        public Enums.AnimalBoolean isVaccinated { get; set; }
        public Enums.AnimalBoolean isSterilized { get; set; }
        public string? description { get; set; }   
        public DateTime arrivedAt { get; set; }
        public List<string>? images { get; set; }
    }
}
