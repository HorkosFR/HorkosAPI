namespace HorkosAPI.Security.Models.Enumerations;

public enum UserTokenResponse
{
    InvalidUserAccessToken,
    ExpiredUserAccessToken,

    InvalidUserRefreshToken,
    ExpiredUserRefreshToken,

    UserTokenMissing,
    WrongTokenFormat,
    NotAcessTypeToken,
    UnableToRenewUserToken,
    RouteUnauthorized
}
