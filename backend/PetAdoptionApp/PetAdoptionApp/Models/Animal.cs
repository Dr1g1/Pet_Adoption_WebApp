using PetAdoptionApp.Common;

namespace PetAdoptionApp.Models
{
    public class Animal
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
        public Boolean isGoodWithKids { get; set; }
        public Boolean isGoodWithPets { get; set; }
        public string? description { get; set; }
        public Boolean isAdopted { get; set; }
        public DateTime arrivedAt { get; set; }
        public List<string>? images { get; set; }
        public List<MedicalRecord>? medicalRecords { get; set; }
        public List<Volunteer> caretakers { get; set; }
        public Shelter shelter { get; set; }
        public Animal? related { get; set; }
    }
}
