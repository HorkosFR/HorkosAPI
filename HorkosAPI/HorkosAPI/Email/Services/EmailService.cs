using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HorkosAPI.Auth.Services;
using HorkosAPI.Database.Models;
using HorkosAPI.Database.Services;
using HorkosAPI.Email.Models;
using HorkosAPI.Email.Models.Enumerations;
using HorkosAPI.Global;
using HorkosAPI.Security.Helpers;
using HorkosAPI.Security.Services;
using HorkosAPI.User.Models.Enumerations;
using Resend;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace HorkosAPI.Email.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IUserTokenService _userTokenService;
    private readonly ConfigurationService _configurationService;
    private readonly DatabaseContext _databaseContext;
    private readonly SymmetricSecurityKey _symmetricSecurityKey;

    public EmailService(IConfiguration configuration, ConfigurationService configurationService, IUserTokenService userTokenService, DatabaseContext databaseContext)
    {
        _configuration = configuration;
        _databaseContext = databaseContext;
        _userTokenService = userTokenService;
        _configurationService = configurationService;
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Application:OAuth2").Value));
    }

    private async void SendEmail(string emailAddress, string emailObject, string emailContent)
    {
        IResend resend = ResendClient.Create(_configuration["Application:ResendToken"]);
        try
        {
            var resp = await resend.EmailSendAsync(new EmailMessage()
            {
                From = "Horkos <no-reply@horkos.fr>",
                To = emailAddress,
                Subject = emailObject,
                HtmlBody = emailContent,
            });
        }
        catch (Exception e)
        {
            throw new Exception(EmailResponse.SendingMailError.ToString(), e);
        }
    }

    public async Task<EmailDto> SendEmailVerificationAsync(EmailDto emailDto, Guid? userId)
    {
        string emailObject = "Inscription sur Horkos";

        string emailToken = await GenerateEmailToken(userId, emailDto.Username, emailDto.Email);

        string appLink = _configuration["FRONT_URL"] + "/verify-email?token=" + emailToken.ToString();

        string emailContent = File.ReadAllText("Email/Templates/email-verification-template.html")
                .Replace("{{VerificationUrl}}", appLink)
                .Replace("{{Username}}", emailDto.Username);

        SendEmail(emailDto.Email, emailObject, emailContent);
        return emailDto;
    }

    public async Task<EmailDto> SendEmailPasswordAsync(ForgotPasswordDto emailDto, Guid? userId)
    {
        string emailObject = "Mise à jour de votre mot de passe Horkos";

        Database.Models.User existingUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Email == emailDto.Email)
                .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());

        string emailToken = await GenerateEmailToken(userId, existingUser.Username, existingUser.Email);

        string appLink = _configuration["FRONT_URL"] + "/update-password?token=" + emailToken.ToString();

        string emailContent = File.ReadAllText("Email/Templates/update-password-template.html")
                .Replace("{{VerificationUrl}}", appLink)
                .Replace("{{Username}}", existingUser.Username);

        SendEmail(emailDto.Email, emailObject, emailContent);
        EmailDto result = new(existingUser.Username, existingUser.Email);
        return result;
    }

    public async Task SendNewEmailVerification(Database.Models.User modifiedUser, string oldEmail)
    {
        string emailToken = await GenerateEmailToken(modifiedUser.Id, modifiedUser.Username, modifiedUser.Email);

        string appLink = _configuration["FRONT_URL"] + "/confirm-email/";

        string emailObject = "Mise à jour de votre email";

        string emailContent = File.ReadAllText("Email/Templates/new-email-verification-template.html")
            .Replace("{{VerificationUrl}}", appLink)
                .Replace("{{Username}}", modifiedUser.Username);

        SendEmail(modifiedUser.Email, emailObject, emailContent);
    }

    private async Task<string> GenerateEmailToken(Guid? userId, string Username, string Email)
    {

        if (userId == null)
        {
            Database.Models.User user = await _databaseContext.Users
                .FirstOrDefaultAsync(u => u.Username == Username)
                .EnsureResultNotNull(UserResponse.UserDoesNotExist.ToString());

            if (user.Email != Email) throw new Exception(UserResponse.WrongEmailForUsername.ToString());

            userId = user.Id;
        }

        IEnumerable<Claim> claims = new List<Claim>() {
                new ("UserId", userId.ToString()),
                new ("Username", Username),
                new ("Email", Email),
                new ("TokenType" , "Access"),
                new (JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds().ToString())
            };

        string emailToken = new JwtSecurityTokenHandler().WriteTokenWithSecurityKey(
            _symmetricSecurityKey,
             "appOrigin",
            claims
         ) ?? throw new Exception(EmailResponse.MailTokenError.ToString());

        return emailToken;
    }
}
