using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Auth.Login.Infrastructure.Extensions;

public static class GitHubLoginStartExtensions
{
    public static void AddGitHubLoginStart(this IServiceCollection services)
    {
        services.AddScoped<GitHubLoginStarter>();
        services.AddSingleton<IOAuthStateIssuer, RandomOAuthStateIssuer>();
        services.AddSingleton<IAuthorizationUrlBuilder, GitHubAuthorizationUrlBuilder>();
    }
}
