using System.ComponentModel.DataAnnotations;

namespace PetAdoptionApp.DTOs.Auth
{
    public class RegisterVolunteerDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8)]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Lozinke se ne poklapaju")]
        public string ConfirmPassword { get; set; }

        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string Address { get; set; }
        public bool HasChildren { get; set; }
        public bool HasPets { get; set; }
        public string? LivingSpace { get; set; }
        public string[]? Skills { get; set; }
        public string[]? AvailableDays { get; set; } //ovo mozda da se napravi da bude Enums
        public string? ShelterId { get; set; }
    }
}
