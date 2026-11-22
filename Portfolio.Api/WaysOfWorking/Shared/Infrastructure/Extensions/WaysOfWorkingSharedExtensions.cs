using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Extensions;

public static class WaysOfWorkingSharedExtensions
{
    public static void AddWaysOfWorkingShared(this IServiceCollection services)
    {
        services.AddScoped<IWayOfWorkingByIdFinderRepository, EntityFrameworkWayOfWorkingByIdFinderAdapter>();
    }
}
