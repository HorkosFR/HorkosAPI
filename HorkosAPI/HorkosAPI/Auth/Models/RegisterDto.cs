using HorkosAPI.Auth.Models.Enumerations;
using HorkosAPI.Database.Models;
using System.Text.RegularExpressions;

namespace HorkosAPI.Auth.Models;

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public RegisterDto(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public RegisterDto TryCheckIsEmptyRequiredValue()
    {
        if (new List<string> { Email, Username, Password }.Any(string.IsNullOrEmpty))
            throw new Exception(RegisterResponse.NullParameters.ToString());
        return this;
    }

    public RegisterDto TryCheckEmailFormat()
    {
        if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            throw new Exception(RegisterResponse.IncorrectEmailFormat.ToString());
        return this;
    }

    public void TryValidate()
    {
        TryCheckIsEmptyRequiredValue().TryCheckEmailFormat();
    }
}
