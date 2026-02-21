using HorkosAPI.Email.Models;
using HorkosAPI.User.Models;

namespace HorkosAPI.User.Services
{
    public interface IUserService
    {
        public Task<List<Database.Models.User>> GetUsersAsync();
        public Task<Database.Models.User?> GetUserByIdAsync(Guid id);
        public Task<Database.Models.User> CreateUserAsync(Database.Models.User user);
        public Task<bool> UpdateUserAsync(Guid id, Database.Models.User updatedUser);
        public Task<bool> DeleteUserAsync(Guid id);
        public Task<Database.Models.User> GetCurrentUserAsync(Database.Models.User user);
        public Task<(Database.Models.User, string? scope)> GetUserByTokenAsync(string token);
        public Task<Database.Models.User> UpdateCurrentUserAsync(Database.Models.User currentUser, UpdateUserDTO updateDto);
        Task<EmailDto> UpdateEmailAsync(UpdateEmailDto emailDto);
        Task<UserDto> UpdatePasswordAsync(PasswordDto passwordDto);
    }
}
