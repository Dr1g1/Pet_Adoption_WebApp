using PetAdoptionApp.DTOs.Auth;

namespace PetAdoptionApp.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginUserAsync(LoginDto dto);
        Task<UserInfoDto> GetUserInfo(string userId, string role, string? shelterId);
        Task<bool> RevokeTokenAsync(string refresh);
        Task<AuthResponseDto> RefreshTokenAsync(string refresh);
    }
}
