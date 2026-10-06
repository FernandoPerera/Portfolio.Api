using Portfolio.Api.Languages.Update.Application.UseCases;
using Portfolio.Api.Languages.Update.Domain.Ports;
using Portfolio.Api.Languages.Update.Infrastructure.Adapters;

namespace Portfolio.Api.Languages.Update.Infrastructure.Extensions;

public static class LanguageUpdateExtensions
{
    public static void AddLanguageUpdate(this IServiceCollection services)
    {
        services.AddScoped<LanguageUpdater>();
        services.AddScoped<ILanguageUpdaterRepository, EntityFrameworkLanguageUpdaterAdapter>();
    }
}
