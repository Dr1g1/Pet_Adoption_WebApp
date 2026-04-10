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
        public Boolean isVaccinated { get; set; }
        public Boolean isSterilized { get; set; }
        public string? description { get; set; }   
        public DateTime arrivedAt { get; set; }
        public List<string>? images { get; set; } //samo prva slika iz liste.
        // ovde mozda stavimo shelter response dto public string shelterId { get; set; }
        public AnimalResponseDto? relative { get; set; }
        public List<MedicalRecord.MedicalRecordResponseDto>? medRecords { get; set; }

    }
}
