namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AuthTokens(AccessToken accessToken, RefreshToken refreshToken)
{
    public string GetAccessToken() => accessToken.ToString();

    public string GetRefreshToken() => refreshToken.ToString();
}
