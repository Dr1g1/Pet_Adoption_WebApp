using PetAdoptionApp.DTOs.Shelter;
using PetAdoptionApp.DTOs.User;

namespace PetAdoptionApp.DTOs.Volunteer
{
    public class VolunteerDto : UserDto
    {
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public float? Rating { get; set; }
        public string[]? Skills { get; set; }
        public string[]? AvailableDays { get; set; }  // enum → string za JSON
        public DateTime? JoinedAt { get; set; }
        public ShelterDto? Shelter { get; set; }
    }
}

