using System.ComponentModel.DataAnnotations;

namespace PetAdoptionApp.DTOs.Auth
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        [Required, EmailAddress] // Data Annotation atributi - ovim dodajemo kao metapodatke nekom polju
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

        [Required, RegularExpression("User|Volonteer", ErrorMessage = "Mora biti korisnik ili volonter.")]
        public string Role { get; set; }

        public string[]? Skills { get; set; }
        public string[]? AvailableDays { get; set; } //ovo mozda da se napravi da bude Enums
        public string? ShelterId { get; set; }
    }
}
