using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.DTOs.MedicalRecord;
using PetAdoptionApp.Interfaces;

namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/medicalRecord")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("{animalId}/medicalrecords")]
        public async Task<IActionResult> GetAnimalMedicalRecords(string animalId)
        {
            var result = await _medicalRecordService.GetMedicalRecordsForAnimal(animalId);
            return Ok(result);
        }

        [HttpDelete("{recordId}")]
        public async Task<IActionResult> DeleteMedicalRecord(string recordId)
        {
            var result = await _medicalRecordService.DeleteRecordAsync(recordId);
            if (!result) return NotFound("Izvestaj sa ovom identifikacijom nije pronadjen");
            return Ok(result);
        }

        [HttpPost("{animalId}")]
        public async Task<IActionResult> CreateMedicalRecord(string animalId, [FromBody] MedicalRecordCreateDto dto)
        {
            var result = await _medicalRecordService.CreateMedicalRecord(animalId, dto);
            return Ok(result);
        }
    }
}
