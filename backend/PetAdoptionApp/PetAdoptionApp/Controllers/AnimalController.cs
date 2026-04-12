using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.DTOs.Animal;
using PetAdoptionApp.Interfaces;
namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/animal")]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        public AnimalController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnimals([FromQuery] string shelterId)
        {
            var result = await _animalService.ReturnAllAnimals(shelterId);
            return Ok(result);
        }

        [HttpGet("{animalId}")] //animalId je deo Url-a, a ne query parametar.
        public async Task<IActionResult> GetAnimalById(string animalId)
        {
            var result = await _animalService.ReturnAnimalId(animalId);
            if (result == null) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimal([FromBody] AnimalCreateDto dto)
        {
            var result = await _animalService.CreateAnimalAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{animalId}")]
        public async Task<IActionResult> DeleteAnimal(string animalId)
        {
            var result = await _animalService.DeleteAnimalAsync(animalId);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }

        [HttpPatch("{animalId}/caretaker")] //patch se koristi kad hocemo da promenimo samo neke delove objekta, ne ceo objekat
        public async Task<IActionResult> AddCaretaker(string animalId, [FromBody] AnimalAddCaretakerDto dto)
        {
            var result = await _animalService.AddCaretakerAsync(animalId, dto);
            if (!result) return NotFound("Zeljeni podaci nisu pronadjeni");
            return Ok(result);
        }

        [HttpPatch("{animalId}/adopt")]
        public async Task<IActionResult> MarkAsAdopted(string animalId, [FromBody] AnimalAdoptedDto dto)
        {
            var result = await _animalService.MarkAsAdoptedAsync(animalId, dto);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }

        [HttpPatch("{animalId}/images")]
        public async Task<IActionResult> UpdateImages(string animalId, [FromBody] AnimalUpdateImagesDto dto)
        {
            var result = await _animalService.UpdateAnimalImages(animalId, dto);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }
    }
}
