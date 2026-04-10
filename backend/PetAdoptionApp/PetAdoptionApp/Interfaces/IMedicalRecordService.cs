using PetAdoptionApp.DTOs;
namespace PetAdoptionApp.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<bool> DeleteRecordAsync(string recordId);
        Task<DTOs.MedicalRecord.MedicalRecordResponseDto> CreateMedicalRecord(string animalId, DTOs.MedicalRecord.MedicalRecordCreateDto dto)
        Task<IEnumerable<DTOs.MedicalRecord.MedicalRecordResponseDto>> ReturnMRForAnimal(string animalId);
    }
}
