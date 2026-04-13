using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.DTOs.AdoptionRequest;
using PetAdoptionApp.Interfaces;

namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/adoptionRequest")]
    public class AdoptionRequestController : ControllerBase
    {
        private readonly IAdoptionRequestService _adoptionRequestService;

        public AdoptionRequestController(IAdoptionRequestService adoptionRequestService)
        {
            _adoptionRequestService = adoptionRequestService;
        }

        [HttpGet("animal/{animalId}")]
        public async Task<IActionResult> GetRequestsForAnimal(string animalId)
        {
            var result = await _adoptionRequestService.GetRequestsForAnimal(animalId);
            return Ok(result);
        }

        [HttpGet("shelter/{shelterId}/pending")]
        public async Task<IActionResult> GetPendingRequestsForShelter(string shelterId)
        {
            var result = await _adoptionRequestService.GetPendingRequests(shelterId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdoptionRequest([FromBody] AdoptionRequestUserCreateDto dto)
        {
            //userId se dobija iz JWT tokena.
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null) return Unauthorized();
            //var userId = "aaf25063-3e47-4673-ba8b-fe55eb526a4a";

            var result = await _adoptionRequestService.CreateAdoptionRequestAsync(userId, dto);
            return Ok(result);
        }

        [HttpDelete("{requestId}")]
        public async Task<IActionResult> DeleteAdoptionRequest(string requestId)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null) return Unauthorized();
            //var userId = "aaf25063-3e47-4673-ba8b-fe55eb526a4a";

            var result = await _adoptionRequestService.DeleteRequestAsync(requestId, userId);
            return Ok(result);
        }

        [HttpPatch("approve")]
        public async Task<IActionResult> ApproveRequest([FromBody] AdoptionRequestActionDto dto)
        {
            var shelterId = User.FindFirst("shelterId")?.Value;
            if (shelterId == null) return Unauthorized();
            //var shelterId = "80936fa3-cdbb-4bb8-b045-3aa37eddd1e3";

            var result = await _adoptionRequestService.ApprovedRequestAsync(dto, shelterId);
            if (!result) return NotFound("Nije pronadjen zahtev");
            return Ok(result);
        }

        [HttpPatch("reject")]
        public async Task<IActionResult> RejectRequest([FromBody] AdoptionRequestActionDto dto)
        {
            var shelterId = User.FindFirst("shelterId")?.Value;
            if (shelterId == null) return Unauthorized();

            var result = await _adoptionRequestService.RejectRequestAsync(dto, shelterId);
            if (!result) return NotFound("Nije pronadjen zahtev");
            return Ok(result);
        }
    }
}
