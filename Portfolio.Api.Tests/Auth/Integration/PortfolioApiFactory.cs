using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Tests.Auth.Integration;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "portfolio-api";
    public const string Audience = "portfolio-clients";
    public const string SigningKey = "a-signing-key-with-at-least-32-bytes!";
    public const string AdminUsername = "admin";
    public const string AdminPassword = "secret";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Jwt:Issuer", Issuer);
        builder.UseSetting("Jwt:Audience", Audience);
        builder.UseSetting("Jwt:SigningKey", SigningKey);
        builder.UseSetting("GitHub:ClientId", "client-id");
        builder.UseSetting("GitHub:ClientSecret", "client-secret");
        builder.UseSetting("GitHub:RedirectUri", "https://portfolio.test/v1/auth/github/callback");
        builder.UseSetting("Auth:FrontendRedirectUri", "https://portfolio.test/login");
        builder.UseSetting("Admin:Username", AdminUsername);
        builder.UseSetting("Admin:PasswordHash", Pbkdf2PasswordHasher.Hash(AdminPassword));
    }
}
