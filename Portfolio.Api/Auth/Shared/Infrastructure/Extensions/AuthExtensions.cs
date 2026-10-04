using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthExtensions
{
    public static void AddAuthSharedInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName);
        services.AddOptions<AdminOptions>().BindConfiguration(AdminOptions.SectionName);

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ICredentialsVerifier, ConfigurationCredentialsVerifier>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IRefreshTokenIssuer, RandomRefreshTokenIssuer>();
        services.AddScoped<IRefreshTokenSaverRepository, EntityFrameworkRefreshTokenSaverAdapter>();
        services.AddScoped<IRefreshTokenConsumerRepository, EntityFrameworkRefreshTokenConsumerAdapter>();
    }
}
