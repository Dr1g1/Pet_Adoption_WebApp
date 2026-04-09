namespace PetAdoptionApp.Models
{
    public class User
    {
        public string id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string? phone { get; set; }
        public string? bio { get; set; }
        public string address { get; set; }
        public Boolean hasChildren { get; set; }
        public Boolean hasPets { get; set; }
        public string? livingSpace { get; set; }
        public List<AdoptionRequest>? adoptionRequests { get; set; }
        public List<Animal>? likedAnimals { get; set; }
        public List<Animal>? adoptedAnimals { get; set; }

    }
}
