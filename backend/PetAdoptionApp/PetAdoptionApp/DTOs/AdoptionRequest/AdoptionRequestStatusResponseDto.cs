using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestStatusResponseDto
    {
        public Enums.Status NewStatus { get; set; }
        public string ResponseMessage { get; set; }
        public string ReviewedById { get; set; } //ne znam da li da ostavim ovo ovde ili ne.
    }
}
