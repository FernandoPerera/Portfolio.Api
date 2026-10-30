using Portfolio.Api.Auth.Logout.Application.UseCases;

namespace Portfolio.Api.Auth.Logout.Infrastructure.Extensions;

public static class SessionCloseExtensions
{
    public static void AddSessionClose(this IServiceCollection services)
    {
        services.AddScoped<SessionCloser>();
    }
}
