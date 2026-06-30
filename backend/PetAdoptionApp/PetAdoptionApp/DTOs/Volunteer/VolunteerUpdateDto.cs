using PetAdoptionApp.DTOs.User;

namespace PetAdoptionApp.DTOs.Volunteer
{
    public class VolunteerUpdateDto : UserUpdateDto
    {
        public bool? IsActive { get; set; }
        public string[]? Skills { get; set; }
        public string[]? AvailableDays { get; set; }
        public string? ShelterId { get; set; }
    }
}
