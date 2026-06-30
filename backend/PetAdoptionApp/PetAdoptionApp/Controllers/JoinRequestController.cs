using Microsoft.AspNetCore.Mvc;
using PetAdoptionApp.DTOs.JoinRequest;
using PetAdoptionApp.Interfaces;

namespace PetAdoptionApp.Controllers
{
	[ApiController]
	[Route("api/joinRequest")]
	public class JoinRequestController : ControllerBase
	{
		private readonly IJoinRequestService _service;

		public JoinRequestController(IJoinRequestService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] JoinRequestCreateDto dto)
		{
			var volunteerId = User.FindFirst("userId")?.Value;
			if (volunteerId == null) return Unauthorized();

			try
			{
				var result = await _service.CreateAsync(volunteerId, dto);
				return Ok(result);
			}
			catch (InvalidOperationException e)
			{
				return Conflict(new { message = e.Message });
			}
		}

		[HttpGet("my")]
		public async Task<IActionResult> GetMyRequests()
		{
			var volunteerId = User.FindFirst("userId")?.Value;
			if (volunteerId == null) return Unauthorized();

			var result = await _service.GetMyRequestsAsync(volunteerId);
			return Ok(result);
		}

		[HttpGet("shelter/{shelterId}/pending")]
		public async Task<IActionResult> GetPendingForShelter(string shelterId)
		{
			var result = await _service.GetPendingForShelterAsync(shelterId);
			return Ok(result);
		}

		[HttpPatch("approve")]
		public async Task<IActionResult> Approve([FromBody] JoinRequestActionDto dto)
		{
			var shelterId = User.FindFirst("shelterId")?.Value;
			var isAdmin = User.FindFirst("isAdmin")?.Value == "true";
			if (shelterId == null || !isAdmin) return Forbid();

			var result = await _service.ApproveAsync(dto.RequestId, shelterId);
			if (!result) return NotFound("Zahtev nije pronađen");
			return Ok(result);
		}

		[HttpPatch("reject")]
		public async Task<IActionResult> Reject([FromBody] JoinRequestActionDto dto)
		{
			var shelterId = User.FindFirst("shelterId")?.Value;
			var isAdmin = User.FindFirst("isAdmin")?.Value == "true";
			if (shelterId == null || !isAdmin) return Forbid();

			var result = await _service.RejectAsync(dto.RequestId, shelterId);
			if (!result) return NotFound("Zahtev nije pronađen");
			return Ok(result);
		}

		[HttpDelete("{requestId}")]
		public async Task<IActionResult> Cancel(string requestId)
		{
			var volunteerId = User.FindFirst("userId")?.Value;
			if (volunteerId == null) return Unauthorized();

			var result = await _service.CancelAsync(requestId, volunteerId);
			if (!result) return NotFound("Zahtev nije pronađen");
			return NoContent();
		}
	}
}