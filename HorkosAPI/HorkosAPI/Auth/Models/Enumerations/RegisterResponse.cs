namespace HorkosAPI.Auth.Models.Enumerations;

public enum RegisterResponse
{
    Ok,
    UsernameAlreadyExists,
    EmailAlreadyExists,
    NullParameters,
    IncorrectParameters,
    RoleError,
    IncorrectUsernameFormat,
    IncorrectNameFormat,
    IncorrectEmailFormat,
    EmailTokenInvalid
}
