using PetAdoptionApp.Common;
namespace PetAdoptionApp.Models
{
    public class Volunteer
    {
        public string id { get; set; }
        public Boolean isAdmin { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string? phone { get; set; }
        public string? bio { get; set; }
        public string address { get; set; }
        public Boolean isActive { get; set; }
        public float? rating { get; set; }
        public string[]? skills { get; set; }
        public Enums.Days[] availableDays { get; set; }
        public DateTime joinedAd { get; set; }
        public Shelter shelter { get; set; }
    }
}
