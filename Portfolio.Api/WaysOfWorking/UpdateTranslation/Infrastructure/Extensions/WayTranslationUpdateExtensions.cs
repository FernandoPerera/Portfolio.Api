using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Ports;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Extensions;

public static class WayTranslationUpdateExtensions
{
    public static void AddWayTranslationUpdate(this IServiceCollection services)
    {
        services.AddScoped<WayTranslationUpdater>();
        services.AddScoped<IWayTranslationUpdaterRepository, EntityFrameworkWayTranslationUpdaterAdapter>();
    }
}
