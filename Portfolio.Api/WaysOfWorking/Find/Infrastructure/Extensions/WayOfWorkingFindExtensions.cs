using Portfolio.Api.WaysOfWorking.Find.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Find.Infrastructure.Extensions;

public static class WayOfWorkingFindExtensions
{
    public static void AddWayOfWorkingFind(this IServiceCollection services)
    {
        services.AddScoped<WayOfWorkingFinder>();
        services.AddScoped<IWayOfWorkingFinderRepository, EntityFrameworkWayOfWorkingFinderAdapter>();
    }
}
