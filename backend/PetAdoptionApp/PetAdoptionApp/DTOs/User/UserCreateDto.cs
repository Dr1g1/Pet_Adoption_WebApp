namespace PetAdoptionApp.DTOs.User
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }    // plain, servis hešuje
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string Address { get; set; }
        public bool HasChildren { get; set; }
        public bool HasPets { get; set; }
        public string? LivingSpace { get; set; }
    }
}
