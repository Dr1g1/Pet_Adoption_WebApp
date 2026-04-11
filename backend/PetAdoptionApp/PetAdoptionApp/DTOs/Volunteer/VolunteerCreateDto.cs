using PetAdoptionApp.DTOs.User;

namespace PetAdoptionApp.DTOs.Volunteer
{
    public class VolunteerCreateDto : UserCreateDto
    {
        public string[]? Skills { get; set; }
        public string[]? AvailableDays { get; set; }
        public string? ShelterId { get; set; }
    }
}
