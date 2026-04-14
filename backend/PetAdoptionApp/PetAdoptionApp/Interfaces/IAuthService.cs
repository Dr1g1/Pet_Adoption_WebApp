using PetAdoptionApp.DTOs.Auth;

namespace PetAdoptionApp.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(RegisterUserDto dto);
        Task<AuthResponseDto> RegisterVolunteerAsync(RegisterVolunteerDto dto);
        Task<AuthResponseDto> LoginUserAsync(LoginDto dto);
        Task<UserInfoDto> GetUserInfo(string userId, string role, string? shelterId);
    }
}
