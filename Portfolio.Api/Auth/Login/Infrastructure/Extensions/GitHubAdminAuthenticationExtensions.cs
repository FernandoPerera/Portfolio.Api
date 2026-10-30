using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Auth.Login.Infrastructure.Extensions;

public static class GitHubAdminAuthenticationExtensions
{
    public static void AddGitHubAdminAuthentication(this IServiceCollection services)
    {
        services.AddScoped<GitHubAdminAuthenticator>();
        services.AddHttpClient(GitHubIdentityProvider.ClientName);
        services.AddScoped<IExternalIdentityProvider, GitHubIdentityProvider>();
        services.AddSingleton<IAdminAuthorizer, ConfigurationAdminAuthorizer>();
        services.AddSingleton<ILoginCodeIssuer, RandomLoginCodeIssuer>();
        services.AddSingleton<ILoginCodeSaverRepository, InMemoryLoginCodeSaverAdapter>();
    }
}
