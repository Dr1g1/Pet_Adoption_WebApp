using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.DTOs.Shelter;
using PetAdoptionApp.Interfaces;

namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/shelter")]
    public class ShelterController : ControllerBase
    {
        private readonly IShelterService _shelterService;

        public ShelterController(IShelterService shelterService)
        {
            _shelterService = shelterService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShelter([FromBody] ShelterDto dto)
        {
            var result = await _shelterService.CreateShelterAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = result.id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shelterService.GetAllSheltersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _shelterService.GetShelterByIdAsync(id);
            if (result == null) return NotFound(); // vraca 404 ako azil ne postoji
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShelter(string id, [FromBody] ShelterCreateDto dto)
        {
            var result = await _shelterService.UpdateShelterAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShelter(string id)
        {
            var success = await _shelterService.DeleteShelterAsync(id);
            if (!success) return NotFound();
            return NoContent(); // vraca 204 - uspesno ali nema sadrzaj za povratak
        }
    }
}
