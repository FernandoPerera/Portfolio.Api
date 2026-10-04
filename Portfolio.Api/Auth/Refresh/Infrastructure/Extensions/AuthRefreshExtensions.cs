using Portfolio.Api.Auth.Refresh.Application.UseCases;

namespace Portfolio.Api.Auth.Refresh.Infrastructure.Extensions;

public static class AuthRefreshExtensions
{
    public static void AddAuthRefresh(this IServiceCollection services)
    {
        services.AddScoped<TokenRefresher>();
    }
}
