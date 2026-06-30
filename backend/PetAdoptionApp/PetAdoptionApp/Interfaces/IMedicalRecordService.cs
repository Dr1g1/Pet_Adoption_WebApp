using PetAdoptionApp.DTOs.MedicalRecord;
namespace PetAdoptionApp.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<bool> DeleteRecordAsync(string recordId);
        Task<MedicalRecordResponseDto> CreateMedicalRecord(string animalId, MedicalRecordCreateDto dto);
        Task<IEnumerable<MedicalRecordResponseDto>> GetMedicalRecordsForAnimal(string animalId);
    }
}
