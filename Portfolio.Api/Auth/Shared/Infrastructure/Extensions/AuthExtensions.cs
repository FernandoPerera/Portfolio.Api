using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Logout.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthExtensions
{
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthOptions(configuration);
        services.AddAuthAdapters();
        services.AddAuthUseCases();
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

    private static void AddAuthAdapters(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IAdminAuthorizer, ConfigurationAdminAuthorizer>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddSingleton<IRefreshTokenIssuer, RandomRefreshTokenIssuer>();
        services.AddSingleton<IRefreshTokenHasher, Sha256RefreshTokenHasher>();
        services.AddSingleton<ILoginCodeIssuer, RandomLoginCodeIssuer>();
        services.AddSingleton<IAdminUsernameProvider, ConfigurationAdminUsernameProvider>();
        services.AddSingleton<IOAuthStateIssuer, RandomOAuthStateIssuer>();
        services.AddSingleton<IAuthorizationUrlBuilder, GitHubAuthorizationUrlBuilder>();
        services.AddHttpClient(GitHubIdentityProvider.ClientName);
        services.AddScoped<IExternalIdentityProvider, GitHubIdentityProvider>();
        services.AddScoped<IRefreshTokenSaverRepository, EntityFrameworkRefreshTokenSaverAdapter>();
        services.AddScoped<IRefreshTokenFinderRepository, EntityFrameworkRefreshTokenFinderAdapter>();
        services.AddScoped<IRefreshTokenConsumerRepository, EntityFrameworkRefreshTokenConsumerAdapter>();
        services.AddScoped<IRefreshTokenRevokerRepository, EntityFrameworkRefreshTokenRevokerAdapter>();
        services.AddScoped<IExpiredRefreshTokenCleanerRepository, EntityFrameworkExpiredRefreshTokenCleanerAdapter>();
        services.AddSingleton<ILoginCodeSaverRepository, InMemoryLoginCodeSaverAdapter>();
        services.AddSingleton<ILoginCodeConsumerRepository, InMemoryLoginCodeConsumerAdapter>();
    }

    private static void AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<TokenRefresher>();
        services.AddScoped<SessionCloser>();
        services.AddScoped<GitHubLoginStarter>();
        services.AddScoped<GitHubAdminAuthenticator>();
        services.AddScoped<LoginCodeExchanger>();
    }
}
