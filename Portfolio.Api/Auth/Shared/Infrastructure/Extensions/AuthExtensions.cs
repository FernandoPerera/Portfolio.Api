using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthExtensions
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<AdminOptions>().Bind(configuration.GetSection(AdminOptions.SectionName));

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

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ICredentialsVerifier, ConfigurationCredentialsVerifier>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IRefreshTokenIssuer, RandomRefreshTokenIssuer>();
        services.AddScoped<IRefreshTokenSaverRepository, EntityFrameworkRefreshTokenSaverAdapter>();
        services.AddScoped<IRefreshTokenConsumerRepository, EntityFrameworkRefreshTokenConsumerAdapter>();

        services.AddScoped<Authenticator>();
        services.AddScoped<TokenRefresher>();
    }
}
