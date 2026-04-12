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

        [HttpPost("{animalId}/addimage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImage(string animalId, IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest("Fajl je obavezan!");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Dozvoljeni formati: jpg, jpeg, png, webp.");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                return BadRequest("Maksimalna velicina fajla je 5MB!");

            var result = await _animalService.AddImageAsync(animalId, file);

            if (result == null)
                return BadRequest("Zivotinja nije pronadjena ili vec ima 5 slika!");

            return Ok(new { path = result });
        }

        [HttpDelete("{animalId}/images/{fileName}")]
        public async Task<IActionResult> RemoveImage(string animalId, string fileName)
        {
            var result = await _animalService.RemoveImageAsync(animalId, fileName);
            return result ? NoContent() : NotFound();
        }

        [HttpPatch("{animalId}/related")]
        public async Task<IActionResult> AddRelatedAnimal(string animalId, [FromBody] AnimalAddRelativeDto dto)
        {
            var result = await _animalService.AddRelativeToAnimal(animalId, dto);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }
    }
}
