using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Tests.Auth.Integration;

public class PortfolioApiFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "portfolio-api";
    public const string Audience = "portfolio-clients";
    public const string SigningKey = "a-signing-key-with-at-least-32-bytes!";
    public const string AdminUsername = "admin";
    public const string AdminGitHubUserId = "12345";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Database:ConnectionString", "Host=unused");
        builder.UseSetting("Jwt:Issuer", Issuer);
        builder.UseSetting("Jwt:Audience", Audience);
        builder.UseSetting("Jwt:SigningKey", SigningKey);
        builder.UseSetting("GitHub:ClientId", "client-id");
        builder.UseSetting("GitHub:ClientSecret", "client-secret");
        builder.UseSetting("GitHub:RedirectUri", "https://portfolio.test/v1/auth/github/callback");
        builder.UseSetting("Auth:FrontendRedirectUri", "https://portfolio.test/login");
        builder.UseSetting("Admin:Username", AdminUsername);
        builder.UseSetting("Admin:GitHubUserId", AdminGitHubUserId);
        builder.UseSetting("RateLimiting:Auth:PermitLimit", "1000");
        builder.UseSetting("RateLimiting:Public:PermitLimit", "1000");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("PortfolioDb"));
        });
    }
}
