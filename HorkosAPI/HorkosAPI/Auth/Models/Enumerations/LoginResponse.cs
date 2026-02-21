namespace HorkosAPI.Auth.Models.Enumerations;

public enum LoginResponse
{
    Ok,
    UsernameNotExisting,
    WrongPassword,
    NullParameters,
    RegistrationNotSucceeded
}
