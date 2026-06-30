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
        public  Enums.AnimalBoolean isVaccinated { get; set; }
        public  Enums.AnimalBoolean isSterilized { get; set; }
        public Enums.AnimalBoolean isGoodWithKids { get; set; }
        public Enums.AnimalBoolean isGoodWithPets { get; set; }
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
