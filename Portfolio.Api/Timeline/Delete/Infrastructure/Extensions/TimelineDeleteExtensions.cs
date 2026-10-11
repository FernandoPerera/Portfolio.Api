using Portfolio.Api.Timeline.Delete.Application.UseCases;
using Portfolio.Api.Timeline.Delete.Domain.Ports;
using Portfolio.Api.Timeline.Delete.Infrastructure.Adapters;

namespace Portfolio.Api.Timeline.Delete.Infrastructure.Extensions;

public static class TimelineDeleteExtensions
{
    public static void AddTimelineDelete(this IServiceCollection services)
    {
        services.AddScoped<TimelineDeleter>();
        services.AddScoped<ITimelineDeleterRepository, EntityFrameworkTimelineDeleterAdapter>();
    }
}
