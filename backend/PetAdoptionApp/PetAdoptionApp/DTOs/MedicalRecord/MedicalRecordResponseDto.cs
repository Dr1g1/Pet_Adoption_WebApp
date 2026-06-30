namespace PetAdoptionApp.DTOs.MedicalRecord
{
    public class MedicalRecordResponseDto
    {
        public string id { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public string clinicPhone { get; set; }
        public string vetName { get; set; }
        public DateTime nextDueDate { get; set; }
        public string[]? vaccines { get; set; }
    }
}
