using App_Proyecto.DTOS;

namespace App_Proyecto.Extensions;

internal static class AuthSessionCookies
{
    public const string Access = "access_token";
    public const string Refresh = "refreshToken";

    public static void Set(HttpResponse response, LoginResponseDto session)
    {
        response.Cookies.Append(Access, session.AccessToken, Options(session.ExpiresAt));
        response.Cookies.Append(Refresh, session.RefreshToken, Options(session.RefreshTokenExpiresAt));
    }

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(Access, Options(null));
        response.Cookies.Delete(Refresh, Options(null));
    }

    private static CookieOptions Options(DateTime? expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = expires
    };
}
