using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthExtensions
{
    public static void AddAuthShared(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthOptions(configuration);
        services.AddAuthSharedAdapters();
    }

    private static void AddAuthOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<AuthOptions>>().Value);
        services.AddOptions<GitHubOptions>()
            .Bind(configuration.GetSection(GitHubOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<GitHubOptions>>().Value);
    }

    private static void AddAuthSharedAdapters(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IRefreshTokenIssuer, RandomRefreshTokenIssuer>();
        services.AddSingleton<IRefreshTokenHasher, Sha256RefreshTokenHasher>();
        services.AddScoped<IRefreshTokenSaverRepository, EntityFrameworkRefreshTokenSaverAdapter>();
        services.AddScoped<IRefreshTokenFinderRepository, EntityFrameworkRefreshTokenFinderAdapter>();
        services.AddScoped<IRefreshTokenRevokerRepository, EntityFrameworkRefreshTokenRevokerAdapter>();
        services.AddScoped<IExpiredRefreshTokenCleanerRepository, EntityFrameworkExpiredRefreshTokenCleanerAdapter>();
    }
}
