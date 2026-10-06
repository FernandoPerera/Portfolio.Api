using Portfolio.Api.Timeline.Update.Application.UseCases;
using Portfolio.Api.Timeline.Update.Domain.Ports;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;

namespace Portfolio.Api.Timeline.Update.Infrastructure.Extensions;

public static class TimelineUpdateExtensions
{
    public static void AddTimelineUpdate(this IServiceCollection services)
    {
        services.AddScoped<TimelineUpdater>();
        services.AddScoped<ITimelineEntryFinderRepository, EntityFrameworkTimelineEntryFinderAdapter>();
        services.AddScoped<ITimelineUpdaterRepository, EntityFrameworkTimelineUpdaterAdapter>();
    }
}
