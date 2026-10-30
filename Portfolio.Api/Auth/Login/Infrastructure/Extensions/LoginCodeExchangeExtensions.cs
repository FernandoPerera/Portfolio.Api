using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Auth.Login.Infrastructure.Extensions;

public static class LoginCodeExchangeExtensions
{
    public static void AddLoginCodeExchange(this IServiceCollection services)
    {
        services.AddScoped<LoginCodeExchanger>();
        services.AddSingleton<IAdminUsernameProvider, ConfigurationAdminUsernameProvider>();
        services.AddSingleton<ILoginCodeConsumerRepository, InMemoryLoginCodeConsumerAdapter>();
    }
}
