using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestRejectionDto
    {
        public Enums.Status rejectionStatus { get; set; }
        public string message { get; set; }
        public string reviewedById { get; set; }
    }
}
