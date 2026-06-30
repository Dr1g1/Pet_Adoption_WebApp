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
            var result = await _animalService.GetAllAnimalsAsync(shelterId);
            return Ok(result);
        }

        [HttpGet("{animalId}")]
        public async Task<IActionResult> GetAnimalById(string animalId)
        {
            var result = await _animalService.GetAnimalByIdAsync(animalId);
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

        [HttpPatch("{animalId}/caretaker")]
        public async Task<IActionResult> AddCaretaker(string animalId, [FromBody] AnimalAddCaretakerDto dto)
        {
            var result = await _animalService.AddCaretakerAsync(animalId, dto.CaretakerId);
            if (!result) return NotFound("Zeljeni podaci nisu pronadjeni");
            return Ok(result);
        }

        [HttpPatch("{animalId}/adopt")]
        public async Task<IActionResult> MarkAsAdopted(string animalId, [FromBody] AnimalAdoptedDto dto)
        {
            var result = await _animalService.MarkAsAdoptedAsync(animalId, dto.UserId);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }

        [HttpPost("{animalId}/images2")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImage(string animalId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fajl je obavezan!");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".jfif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Dozvoljeni formati: jpg, jpeg, png, webp.");

            if (file.Length > 5 * 1024 * 1024) 
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
            var result = await _animalService.AddRelativeAsync(animalId, dto.RelativeId);
            if (!result) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllAvailableAnimals()
        {
            var result = await _animalService.GetAllAvailableAnimalsAsync();
            return Ok(result);
        }

        [HttpGet("shelter/{shelterId}/all")]
        public async Task<IActionResult> GetAllByShelter(string shelterId)
        {
            var result = await _animalService.GetAllByShelterIncludingAdoptedAsync(shelterId);
            return Ok(result);
        }

        [HttpPatch("{animalId}")]
        public async Task<IActionResult> UpdateAnimal(string animalId, [FromBody] AnimalUpdateDto dto)
        {
            var result = await _animalService.UpdateAnimalAsync(animalId, dto);
            if (result == null) return NotFound("Zivotinja sa ovom identifikacijom nije pronadjena");
            return Ok(result);
        }

        [HttpPost("{animalId}/like")]
        public async Task<IActionResult> LikeAnimal(string animalId)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null) return Unauthorized();

            var result = await _animalService.LikeAnimalAsync(userId, animalId);
            if (!result) return NotFound("Zivotinja nije pronadjena");
            return Ok(result);
        }

        [HttpDelete("{animalId}/like")]
        public async Task<IActionResult> UnlikeAnimal(string animalId)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null) return Unauthorized();

            var result = await _animalService.UnlikeAnimalAsync(userId, animalId);
            return Ok(result);
        }

        [HttpGet("liked")]
        public async Task<IActionResult> GetLikedAnimals()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null) return Unauthorized();

            var result = await _animalService.GetLikedAnimalsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{animalId}/likes/count")]
        public async Task<IActionResult> GetLikeCount(string animalId)
        {
            var result = await _animalService.GetLikeCountAsync(animalId);
            return Ok(new { count = result });
        }
    }
}