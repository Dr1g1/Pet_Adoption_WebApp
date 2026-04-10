namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestUserCreateDto
    {
        //dto za korisnika koji salje zhatev za usvajanjem neke zivotinje.
        public string animalId { get; set; }
        public string message { get; set; }
    }
}
