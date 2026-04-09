using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestStatusUpdateDto
    {
        public Enums.Status newStatus { get; set; }
        public string reviewedById { get; set; } //ne znam da li da ostavim ovo ovde ili ne.
    }
}
