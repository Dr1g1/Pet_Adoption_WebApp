using PetAdoptionApp.DTOs.User;

namespace PetAdoptionApp.DTOs.Volunteer
{
    public class VolunteerSummaryDto : UserSummaryDto
    {
        public bool IsActive { get; set; }
        public float? Rating { get; set; }
        public string? ShelterName { get; set; }
    }
}
