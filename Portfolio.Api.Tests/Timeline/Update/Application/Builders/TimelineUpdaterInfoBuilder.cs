using Portfolio.Api.Timeline.Create.Application.Dtos;
using Portfolio.Api.Timeline.Update.Application.Dtos;

namespace Portfolio.Api.Tests.Timeline.Update.Application.Builders;

public abstract class TimelineUpdaterInfoBuilder
{
    public static TimelineUpdaterInfo Build(
        Guid? id = null,
        string? organization = null,
        string? category = null,
        string? startDate = null,
        string? endDate = null,
        List<string>? technologies = null,
        List<TimelineTranslationInfo>? translations = null
    )
    {
        return new TimelineUpdaterInfo(
            id ?? Guid.NewGuid(),
            organization,
            category,
            startDate,
            endDate,
            technologies,
            translations
        );
    }
}
