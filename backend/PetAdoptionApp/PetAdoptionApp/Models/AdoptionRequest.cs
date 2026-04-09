using Microsoft.AspNetCore.Mvc;

namespace PetAdoptionApp.Models
{
    public enum Status { Pending, Approved, Rejected }
    public class AdoptionRequest
    {
        public string id { get; set; }
        public Status status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string? message { get; set; }
        public Animal animal { get; set; }
        public Shelter? reviewedBy { get; set; }
    }
}
