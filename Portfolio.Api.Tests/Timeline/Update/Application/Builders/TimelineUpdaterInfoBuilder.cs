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
        string? title = null,
        string? description = null,
        string languageCode = "es"
    )
    {
        return new TimelineUpdaterInfo(
            id ?? Guid.NewGuid(),
            organization,
            category,
            startDate,
            endDate,
            technologies,
            title,
            description,
            languageCode
        );
    }
}
