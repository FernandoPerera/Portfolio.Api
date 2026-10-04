using Portfolio.Api.Timeline.Find.Application.UseCases;
using Portfolio.Api.Timeline.Find.Domain.Ports;
using Portfolio.Api.Timeline.Find.Infrastructure.Adapters;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Extensions;

public static class TimelineFindExtensions
{
    public static void AddTimelineFind(this IServiceCollection services)
    {
        services.AddScoped<TimelineFinder>();
        services.AddScoped<ITimelineFinderRepository, EntityFrameworkTimelineFinderAdapter>();
    }
}
