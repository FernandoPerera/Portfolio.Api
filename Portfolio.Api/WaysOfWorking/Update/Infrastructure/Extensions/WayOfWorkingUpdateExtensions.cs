using Portfolio.Api.WaysOfWorking.Update.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Update.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Update.Infrastructure.Extensions;

public static class WayOfWorkingUpdateExtensions
{
    public static void AddWayOfWorkingUpdate(this IServiceCollection services)
    {
        services.AddScoped<WayOfWorkingUpdater>();
        services.AddScoped<IWayOfWorkingUpdaterRepository, EntityFrameworkWayOfWorkingUpdaterAdapter>();
    }
}
