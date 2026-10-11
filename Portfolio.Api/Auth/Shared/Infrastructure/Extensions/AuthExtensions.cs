using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Authorization;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthExtensions
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthOptions(configuration);
        services.AddJwtBearerAuthentication();
        services.AddAdminAuthorization();
        services.AddAuthAdapters();
        services.AddAuthUseCases();
    }

    private static void AddAuthOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<AdminOptions>().Bind(configuration.GetSection(AdminOptions.SectionName));
        services.AddOptions<GitHubOptions>().Bind(configuration.GetSection(GitHubOptions.SectionName));
    }

    private static void AddJwtBearerAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                bearerOptions.MapInboundClaims = false;
                bearerOptions.Events = new ProblemDetailsJwtBearerEvents();
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
    }

    private static void AddAdminAuthorization(this IServiceCollection services)
    {
        var adminPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(AdminAccess.RoleClaimType, AdminAccess.RoleClaimValue)
            .Build();

        services.AddAuthorizationBuilder()
            .AddPolicy(AdminAccess.PolicyName, adminPolicy)
            .SetDefaultPolicy(adminPolicy)
            .SetFallbackPolicy(adminPolicy);
    }

    private static void AddAuthAdapters(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ICredentialsVerifier, ConfigurationCredentialsVerifier>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IRefreshTokenIssuer, RandomRefreshTokenIssuer>();
        services.AddHttpClient(GitHubIdentityProvider.ClientName);
        services.AddScoped<IExternalIdentityProvider, GitHubIdentityProvider>();
        services.AddScoped<IRefreshTokenSaverRepository, EntityFrameworkRefreshTokenSaverAdapter>();
        services.AddScoped<IRefreshTokenConsumerRepository, EntityFrameworkRefreshTokenConsumerAdapter>();
    }

    private static void AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<Authenticator>();
        services.AddScoped<TokenRefresher>();
    }
}
