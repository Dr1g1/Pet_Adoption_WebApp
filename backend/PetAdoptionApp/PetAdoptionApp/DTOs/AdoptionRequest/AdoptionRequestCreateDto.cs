namespace PetAdoptionApp.DTOs.AdoptionRequest
{
    public class AdoptionRequestCreateDto
    {
        //dto za korisnika koji salje zhatev za usvajanjem neke zivotinje.
        public string animalId { get; set; }
        public string message { get; set; }
    }
}
