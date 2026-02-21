using HorkosAPI.Auth.Models;
using HorkosAPI.Email.Models;
using HorkosAPI.User.Models;

namespace HorkosAPI.Auth.Services;

public interface IAuthService
{
    Task<Database.Models.User> RegisterUserAsync(RegisterDto registerDto);

    Task<LoggedUserDto> LoginUserAsync(LoginDto loginDto);

    Task<EmailDto> VerifyAccountAsync(VerificationDto verificationDto);
    Task<Database.Models.User> ResendEmailAsync(RegisterDto registerDto);
}
