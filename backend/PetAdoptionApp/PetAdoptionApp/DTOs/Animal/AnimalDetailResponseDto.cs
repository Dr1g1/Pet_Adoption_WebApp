using PetAdoptionApp.DTOs.Shelter;
using PetAdoptionApp.DTOs.MedicalRecord;
namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalDetailResponseDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string species { get; set; }
        public string breed { get; set; }
        public int? age { get; set; }
        public string gender { get; set; }
        public string size { get; set; }
        public bool isVaccinated { get; set; }
        public bool isSterilized { get; set; }
        public string? description { get; set; }   
        public DateTime arrivedAt { get; set; }
        public List<string>? images { get; set; } 
        public ShelterDto Shelter { get; set; }
        public AnimalResponseDto? relative { get; set; }
        public List<MedicalRecordResponseDto>? medRecords { get; set; }

    }
}
