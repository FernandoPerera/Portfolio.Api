using Portfolio.Api.WaysOfWorking.Reorder.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Reorder.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Extensions;

public static class WayOrderUpdateExtensions
{
    public static void AddWayOrderUpdate(this IServiceCollection services)
    {
        services.AddScoped<WayOrderUpdater>();
        services.AddScoped<IWayOrderRepository, EntityFrameworkWayOrderAdapter>();
    }
}
