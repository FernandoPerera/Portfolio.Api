namespace Portfolio.Api.Auth.Login.Infrastructure;

public static class OAuthStateCookie
{
    public const string Name = "oauth_state";
    public const string Path = "/v1/auth/github";
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);
}
