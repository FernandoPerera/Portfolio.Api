using Portfolio.Api.WaysOfWorking.AddTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Ports;
using Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Extensions;

public static class WayTranslationAddExtensions
{
    public static void AddWayTranslationAdd(this IServiceCollection services)
    {
        services.AddScoped<WayTranslationAdder>();
        services.AddScoped<IWayTranslationAdderRepository, EntityFrameworkWayTranslationAdderAdapter>();
    }
}
