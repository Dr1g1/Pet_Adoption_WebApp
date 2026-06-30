using PetAdoptionApp.Common;
namespace PetAdoptionApp.Models
{
    public class Volunteer : User
    {
        public Boolean isAdmin { get; set; }
        public Boolean isActive { get; set; }
        public float? rating { get; set; }
        public string[]? skills { get; set; }
        public Enums.Days[]? availableDays { get; set; }
        public DateTime? joinedAd { get; set; }
        public Shelter? shelter { get; set; }
    }
}
