using PetAdoptionApp.Common;

namespace PetAdoptionApp.DTOs.JoinRequest
{
    public class JoinRequestReturnDto
    {
        public string RequestId { get; set; }
        public string VolunteerId { get; set; }
        public string VolunteerName { get; set; }
        public string ShelterId { get; set; }
        public string ShelterName { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Message { get; set; }
    }
}