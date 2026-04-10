namespace PetAdoptionApp.DTOs.Shelter
{
    public class ShelterDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int capacity { get; set; }
        public float? rating { get; set; }
        public string? description { get; set; }
    }
}
