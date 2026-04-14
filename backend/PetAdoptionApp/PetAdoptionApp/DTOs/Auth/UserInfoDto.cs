namespace PetAdoptionApp.DTOs.Auth
{
    public class UserInfoDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? ShelterId { get; set; } // potrebno za volontere i admine azila.
    }
}
