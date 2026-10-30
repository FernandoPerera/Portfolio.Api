using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Auth.Refresh.Infrastructure.Extensions;

public static class TokenRefreshExtensions
{
    public static void AddTokenRefresh(this IServiceCollection services)
    {
        services.AddScoped<TokenRefresher>();
        services.AddScoped<IRefreshTokenConsumerRepository, EntityFrameworkRefreshTokenConsumerAdapter>();
    }
}
