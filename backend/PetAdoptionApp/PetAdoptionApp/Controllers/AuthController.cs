using PetAdoptionApp.DTOs.Auth;
using PetAdoptionApp.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace PetAdoptionApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginUserAsync(dto);
                return Ok(result);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}
