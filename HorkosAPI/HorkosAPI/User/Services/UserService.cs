using Microsoft.EntityFrameworkCore;
using HorkosAPI.Auth.Models.Enumerations;
using HorkosAPI.Database.Services;
using HorkosAPI.Email.Models;
using HorkosAPI.Global;
using HorkosAPI.Security.Models;
using HorkosAPI.User.Models;
using HorkosAPI.User.Models.Enumerations;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Helpers;

namespace HorkosAPI.User.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly Supabase.Client _supabaseClient;

    public UserService(IConfiguration configuration, DatabaseContext context, Supabase.Client supabaseClient)
    {
        _configuration = configuration;
        _context = context;
        _supabaseClient = supabaseClient;
    }

    public async Task<List<Database.Models.User>> GetUsersAsync() =>
        await _context.Users
                      .Include(u => u.Contributions.OrderByDescending(c => c.Timestamp).Take(10))
                      .Include(u => u.Comments)
                      .Include(u => u.Points)
                      .Include(u => u.Badges)
                      .ToListAsync();
    

    public async Task<Database.Models.User?> GetUserByIdAsync(Guid id) =>
        await _context.Users
                      .Include(u => u.Contributions.OrderByDescending(c => c.Timestamp).Take(10))
                      .Include(u => u.Comments)
                      .Include(u => u.Points)
                      .Include(u => u.Badges)
                      .FirstOrDefaultAsync(u => u.Id == id);
    

    public async Task<Database.Models.User> CreateUserAsync(Database.Models.User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }


    public async Task<bool> UpdateUserAsync(Guid id, Database.Models.User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        if (!string.IsNullOrEmpty(updatedUser.Password))
        {
            var stringPassword = Convert.FromBase64String(updatedUser.Password);
            var hashPassword = Crypto.HashPassword(BitConverter.ToString(stringPassword));
            user.Password = hashPassword;
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<(Database.Models.User, string? scope)> GetUserByTokenAsync(string token)
    {
        JwtSecurityToken securityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);
        string userId = securityToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? throw new Exception(UserResponse.TokenInvalid.ToString());
        string? scope = securityToken.Claims.FirstOrDefault(x => x.Type == "Scope")?.Value;
        Database.Models.User u = await _context.Users
            .Include(x => x.Role)
            .Include(u => u.Contributions)
            .Include(u => u.Comments)
            .Include(u => u.Points)
            .Include(u => u.Badges)
            .FirstOrDefaultAsync(x => x.Id.Equals(Guid.Parse(userId)))
            .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());
        return (u, scope);
    }
    

    public async Task<Database.Models.User> GetCurrentUserAsync(Database.Models.User user)
    {
        if (user.AccountStatus != "Verified")
            throw new Exception(UserResponse.UserDoesNotExist.ToString());
        return user;
    }
    

    public async Task<Database.Models.User> UpdateCurrentUserAsync(Database.Models.User user, UpdateUserDTO updateDto)
    {
        user.Username = updateDto.Username ?? user.Username;
        user.Email = updateDto.Email ?? user.Email;
        user.Locale = updateDto.Locale ?? user.Locale;
        if (!string.IsNullOrEmpty(updateDto.Password))
        {
            var stringPassword = Convert.FromBase64String(updateDto.Password);
            var hashPassword = Crypto.HashPassword(BitConverter.ToString(stringPassword));
            user.Password = hashPassword;
        }
        if (!string.IsNullOrEmpty(updateDto.Image))
        {
            var base64Data = updateDto.Image.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            string fileName = $"user_{user.Id}.webp";
            await _supabaseClient.Storage
                .From("avatars")
                .Upload(imageBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
            user.ImageUrl = _supabaseClient.Storage
                .From("avatars")
                .GetPublicUrl(fileName);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<UserDto> UpdatePasswordAsync(PasswordDto passwordDto)
    {
        EmailToken? emailToken = null;
        try
        {
            emailToken = new(passwordDto.Token);
        }
        catch
        {
            throw new Exception(RegisterResponse.EmailTokenInvalid.ToString());
        }

        Database.Models.User existingUser =
            await _context.Users.FirstOrDefaultAsync(x => x.Username == emailToken.Username)
            .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());

        if (existingUser.AccountStatus != "Verified")
            throw new Exception(UserResponse.RegistrationNotCompleted.ToString());

        var stringPassword = Convert.FromBase64String(passwordDto.Password);
        var hashPassword = Crypto.HashPassword(BitConverter.ToString(stringPassword));

        existingUser.Password = hashPassword;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        return new UserDto(existingUser);
    }

    public async Task<EmailDto> UpdateEmailAsync(UpdateEmailDto emailDto)
    {
        EmailToken? emailToken = null;
        try
        {
            emailToken = new(emailDto.Token);
        }
        catch
        {
            throw new Exception(RegisterResponse.EmailTokenInvalid.ToString());
        }

        Database.Models.User existingUser =
            await _context.Users.FirstOrDefaultAsync(x => x.Username == emailToken.Username)
            .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());
        if (existingUser.Email != emailToken.Email)
        {
            existingUser.Email = emailToken.Email;
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception(UserResponse.EmailNotModified.ToString());
        }

        return new EmailDto(emailToken.Username, emailToken.Email);
    }
}
