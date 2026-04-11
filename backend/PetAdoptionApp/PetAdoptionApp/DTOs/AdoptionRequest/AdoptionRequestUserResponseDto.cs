using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestUserResponseDto
    {
        public string RequestId { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AnimalId { get; set; }
        public string ShelterId { get; set; }
    }
}
