using PetAdoptionApp.DTOs.User;

namespace PetAdoptionApp.Interfaces
{
    public interface IUserService
    {
        Task<List<UserSummaryDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto> CreateAsync(UserCreateDto dto);
        Task<UserDto?> UpdateAsync(string id, UserUpdateDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
