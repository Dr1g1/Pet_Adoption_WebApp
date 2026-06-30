namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestUserCreateDto
    {
        //dto za korisnika koji salje zhatev za usvajanje neke zivorinje
        public string animalId { get; set; }
        public string message { get; set; }
    }
}
