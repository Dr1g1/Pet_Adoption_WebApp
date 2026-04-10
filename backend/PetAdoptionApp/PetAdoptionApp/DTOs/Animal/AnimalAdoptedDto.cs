namespace PetAdoptionApp.DTOs.Animal
{
    public class AnimalAdoptedDto
    {
        public bool IsAdopted { get; set; }
        public string AdoptedByUserId { get; set; }
        public DateTime? AdoptionDate { get; set; }
    }
}
