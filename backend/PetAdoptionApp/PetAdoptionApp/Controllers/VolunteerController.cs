using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.DTOs.Volunteer;

namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/volunteers")]
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteerService _service;

        public VolunteerController(IVolunteerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET api/volunteers?shelterId=abc123
        [HttpGet("by-shelter/{shelterId}")]
        public async Task<IActionResult> GetByShelter(string shelterId)
            => Ok(await _service.GetByShelterAsync(shelterId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var volunteer = await _service.GetByIdAsync(id);
            return volunteer == null ? NotFound() : Ok(volunteer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VolunteerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] VolunteerUpdateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // poseban endpoint za dodelu azilu
        [HttpPut("{volunteerId}/assign/{shelterId}")]
        public async Task<IActionResult> AssignToShelter(
            string volunteerId, string shelterId)
        {
            var result = await _service.AssignToShelterAsync(volunteerId, shelterId);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
