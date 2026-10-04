using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Tests.Auth.Integration;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    public const string AdminUsername = "admin";
    public const string AdminPassword = "secret";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Jwt:Issuer", "portfolio-api");
        builder.UseSetting("Jwt:Audience", "portfolio-clients");
        builder.UseSetting("Jwt:SigningKey", "a-signing-key-with-at-least-32-bytes!");
        builder.UseSetting("Admin:Username", AdminUsername);
        builder.UseSetting("Admin:PasswordHash", Pbkdf2PasswordHasher.Hash(AdminPassword));
    }
}
