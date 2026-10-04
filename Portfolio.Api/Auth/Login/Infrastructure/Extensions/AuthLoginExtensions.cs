using Portfolio.Api.Auth.Login.Application.UseCases;

namespace Portfolio.Api.Auth.Login.Infrastructure.Extensions;

public static class AuthLoginExtensions
{
    public static void AddAuthLogin(this IServiceCollection services)
    {
        services.AddScoped<Authenticator>();
    }
}
