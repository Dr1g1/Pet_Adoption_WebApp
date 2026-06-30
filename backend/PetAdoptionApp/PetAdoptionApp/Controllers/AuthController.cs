using PetAdoptionApp.DTOs.Auth;
using PetAdoptionApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


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
                return Unauthorized(new {message = e.Message});
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);
            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch(UnauthorizedAccessException e)
            {
                return Unauthorized(new { message = e.Message });
            }
        }

        //vracanje informacija o trenutno ulogovanom korisniku:
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var shelterId = User.FindFirst("shelterId")?.Value;
            return Ok(new { id, email, role, shelterId });
        }
    }
}
