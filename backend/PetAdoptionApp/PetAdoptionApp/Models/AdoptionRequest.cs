using PetAdoptionApp.Common;
using Microsoft.AspNetCore.Mvc;

namespace PetAdoptionApp.Models
{
    public class AdoptionRequest
    {
        public string id { get; set; }
        public Enums.Status status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? message { get; set; }
        public Animal animal { get; set; }
        public Shelter? reviewedBy { get; set; }
    }
}
