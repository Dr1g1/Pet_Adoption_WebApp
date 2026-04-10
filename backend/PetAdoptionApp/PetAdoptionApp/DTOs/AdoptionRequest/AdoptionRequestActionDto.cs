
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestActionDto
    {
        //ovo je dto za volontere koji odgovaraju na zahteve koji su poslati.
        public string RequestId { get; set; }
        public string NewStatus { get; set; }
        public string ResponseMessage { get; set; }
        public string ReviewedById { get; set; } //ne znam da li da ostavim ovo ovde ili ne.
    }
}
