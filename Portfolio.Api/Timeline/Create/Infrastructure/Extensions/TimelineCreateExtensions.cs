using Portfolio.Api.Timeline.Create.Application.UseCases;
using Portfolio.Api.Timeline.Create.Domain.Ports;
using Portfolio.Api.Timeline.Create.Infrastructure.Adapters;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Extensions;

public static class TimelineCreateExtensions
{
    public static void AddTimelineCreate(this IServiceCollection services)
    {
        services.AddScoped<TimelineCreator>();
        services.AddScoped<ITimelineCreatorRepository, EntityFrameworkTimelineCreatorAdapter>();
    }
}
