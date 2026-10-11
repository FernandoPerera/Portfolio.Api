using Portfolio.Api.Timeline.AddTranslation.Application.UseCases;
using Portfolio.Api.Timeline.AddTranslation.Domain.Ports;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Adapters;

namespace Portfolio.Api.Timeline.AddTranslation.Infrastructure.Extensions;

public static class TimelineTranslationAddExtensions
{
    public static void AddTimelineTranslationAdd(this IServiceCollection services)
    {
        services.AddScoped<TimelineTranslationAdder>();
        services.AddScoped<ITimelineTranslationAdderRepository, EntityFrameworkTimelineTranslationAdderAdapter>();
    }
}
