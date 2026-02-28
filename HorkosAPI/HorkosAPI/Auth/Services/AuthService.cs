using Microsoft.EntityFrameworkCore;
using HorkosAPI.Auth.Models;
using HorkosAPI.Auth.Models.Enumerations;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Email.Models;
using HorkosAPI.Email.Services;
using HorkosAPI.Global;
using HorkosAPI.Security.Models;
using HorkosAPI.Security.Services;
using HorkosAPI.User.Models;
using HorkosAPI.User.Models.Enumerations;
using System.Web.Helpers;

namespace HorkosAPI.Auth.Services;

public class AuthService(DatabaseContext _databaseContext, IUserTokenService userTokenService, IEmailService _emailService) : IAuthService
{
    public async Task<Database.Models.User> RegisterUserAsync(RegisterDto registerDto)
    {
        registerDto.TryValidate();

        await _databaseContext.Users
            .FirstOrDefaultAsync(x => x.Username == registerDto.Username)
            .EnsureResultNull(RegisterResponse.UsernameAlreadyExists.ToString());

        await _databaseContext.Users
            .FirstOrDefaultAsync(x => x.Email == registerDto.Email)
            .EnsureResultNull(RegisterResponse.EmailAlreadyExists.ToString());

        Database.Models.Role role = await _databaseContext.Roles
            .FirstOrDefaultAsync(x => x.Name == RoleValues.User.ToString())
            .EnsureResultNotNull(RegisterResponse.RoleError.ToString());


        Database.Models.User newUser = new(registerDto, role.Id);
        var stringPassword = Convert.FromBase64String(registerDto.Password);
        var hashPassword = Crypto.HashPassword(BitConverter.ToString(stringPassword));
        newUser.Password = hashPassword;

        string userToken = userTokenService.GenerateJwtToken(newUser, null, "Refresh");
        var stringToken = Convert.FromBase64String(userToken);
        var hashToken = Crypto.HashPassword(BitConverter.ToString(stringToken));
        newUser.RefreshToken = hashToken;
        newUser.Locale = "fr";

        _databaseContext.Users.Add(newUser);

        Database.Models.UserPoints userPoints = new(newUser);
        _databaseContext.UserPoints.Add(userPoints);
        try
        {
            await _databaseContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        EmailDto emailSentTo = await _emailService.SendEmailVerificationAsync(new EmailDto(registerDto.Username, registerDto.Email), newUser.Id);

        return newUser;
    }

    public async Task<Database.Models.User> ResendEmailAsync(RegisterDto registerDto)
    {
        Database.Models.User user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == registerDto.Username) ?? throw new Exception(UserResponse.UserDoesNotExist.ToString());
        EmailDto emailSentTo = await _emailService.SendEmailVerificationAsync(new EmailDto(registerDto.Username, registerDto.Email), user.Id);
        return user;
    }


    public async Task<LoggedUserDto> LoginUserAsync(LoginDto loginDto)
    {
        loginDto.TryCheckIsEmptyRequiredValue();

        Database.Models.User user =
            _databaseContext.Users.FirstOrDefault(x => x.Email == loginDto.Email)
            .EnsureNotNull(LoginResponse.UsernameNotExisting.ToString());
        if (!user.AccountStatus.Equals("Verified"))
        {
            throw new Exception(LoginResponse.RegistrationNotSucceeded.ToString());
        }
        var stringPassword = Convert.FromBase64String(loginDto.Password);
        var stringPassword2 = Convert.FromBase64String(user.Password ?? "");
        if (!Crypto.VerifyHashedPassword(user.Password, BitConverter.ToString(stringPassword)))
        {
            var stringPass = BitConverter.ToString(stringPassword);
            throw new Exception(LoginResponse.WrongPassword.ToString());
        }

        string userToken = userTokenService.GenerateJwtToken(user, DateTime.UtcNow.AddHours(2), "Access");

        Database.Models.Role role = _databaseContext.Roles
            .FirstOrDefault(x => x.Id == user.RoleId)
            .EnsureNotNull(UserResponse.UnknownRole.ToString());
        user.LastLogin = DateTime.UtcNow;
        _databaseContext.Users.Update(user);
        await _databaseContext.SaveChangesAsync();
        return new LoggedUserDto(user, role.Name ?? "", userToken);
    }

    public async Task<EmailDto> VerifyAccountAsync(VerificationDto verificationDto)
    {
        EmailToken? emailToken = null;
        try
        {
            emailToken = new(verificationDto.EmailToken);
        }
        catch
        {
            throw new Exception(RegisterResponse.EmailTokenInvalid.ToString());
        }

        Database.Models.User existingUser =
            await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == emailToken.Username)
            .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());

        if (verificationDto.IsFirstVerification)
        {
            existingUser.AccountStatus = "Verified";
            _databaseContext.Users.Update(existingUser);
            await _databaseContext.SaveChangesAsync();
        }

        return new EmailDto(emailToken.Username, emailToken.Email);
    }
}
