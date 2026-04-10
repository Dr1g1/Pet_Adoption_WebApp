namespace PetAdoptionApp.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string Address { get; set; }
        public bool HasChildren { get; set; }
        public bool HasPets { get; set; }
        public string? LivingSpace { get; set; }
        public List<string>? LikedAnimalIds { get; set; }
        public List<string>? AdoptedAnimalIds { get; set; }
    }

}
