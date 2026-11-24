using Portfolio.Api.WaysOfWorking.Create.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Create.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Create.Infrastructure.Extensions;

public static class WayOfWorkingCreateExtensions
{
    public static void AddWayOfWorkingCreate(this IServiceCollection services)
    {
        services.AddScoped<WayOfWorkingCreator>();
        services.AddScoped<IWayOfWorkingCreatorRepository, EntityFrameworkWayOfWorkingCreatorAdapter>();
    }
}
