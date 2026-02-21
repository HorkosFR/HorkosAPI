using HorkosAPI.Auth.Models.Enumerations;

namespace HorkosAPI.Auth.Models;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }

    public LoginDto(string email, string password)
    {
        Email = email;
        Password = password;
    }


    public LoginDto TryCheckIsEmptyRequiredValue()
    {
        if (new List<string> { Email, Password }.Any(string.IsNullOrEmpty))
            throw new Exception(LoginResponse.NullParameters.ToString());
        return this;
    }
}
