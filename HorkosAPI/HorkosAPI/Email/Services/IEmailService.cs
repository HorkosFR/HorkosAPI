using HorkosAPI.Email.Models;
using System.Net.Mail;

namespace HorkosAPI.Email.Services;

public interface IEmailService
{
    Task<EmailDto> SendEmailVerificationAsync(EmailDto emailDto, Guid? userId);

    Task SendNewEmailVerification(Database.Models.User modifiedUser, string oldEmail);
    Task<EmailDto> SendEmailPasswordAsync(ForgotPasswordDto emailDto, Guid? userId);
}
