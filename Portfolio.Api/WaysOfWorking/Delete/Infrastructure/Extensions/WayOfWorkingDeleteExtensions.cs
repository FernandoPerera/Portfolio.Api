using Portfolio.Api.WaysOfWorking.Delete.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Delete.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Extensions;

public static class WayOfWorkingDeleteExtensions
{
    public static void AddWayOfWorkingDelete(this IServiceCollection services)
    {
        services.AddScoped<WayOfWorkingDeleter>();
        services.AddScoped<IWayOfWorkingDeleterRepository, EntityFrameworkWayOfWorkingDeleterAdapter>();
    }
}
