using PetAdoptionApp.Common;
namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestReturnDto
    {
        public string RequestId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AnimalId { get; set; }
        public string AnimalName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
