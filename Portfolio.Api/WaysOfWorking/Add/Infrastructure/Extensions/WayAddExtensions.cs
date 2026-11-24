using Portfolio.Api.WaysOfWorking.Add.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Add.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Add.Infrastructure.Adapters;

namespace Portfolio.Api.WaysOfWorking.Add.Infrastructure.Extensions;

public static class WayAddExtensions
{
    public static void AddWayTranslationAdd(this IServiceCollection services)
    {
        services.AddScoped<WayAdder>();
        services.AddScoped<IWayAdderRepository, EntityFrameworkWayAdderAdapter>();
    }
}
