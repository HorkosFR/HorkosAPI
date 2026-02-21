namespace HorkosAPI.User.Models.Enumerations;

public enum UserResponse
{
    UserDoesNotExist,
    TokenInvalid,
    RegistrationAlreadyCompleted,
    EmailNotVerified,
    PasswordAlreadyInitialized,
    UnknownRole,
    RegistrationNotCompleted,
    InvitationMailResent,
    UserModified,
    UserModifedMailResent,
    WrongEmailForUsername,
    EmailNotModified,
    TokenMissing
}
