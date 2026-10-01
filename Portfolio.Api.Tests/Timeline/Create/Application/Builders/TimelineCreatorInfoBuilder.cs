using Portfolio.Api.Timeline.Create.Application.Dtos;

namespace Portfolio.Api.Tests.Timeline.Create.Application.Builders;

public abstract class TimelineCreatorInfoBuilder
{
    public static TimelineCreatorInfo Build(
        string organization = "Lean Mind",
        string category = "work",
        string startDate = "2023-01",
        string? endDate = null,
        List<string>? technologies = null,
        List<TimelineTranslationInfo>? translations = null
    )
    {
        return new TimelineCreatorInfo(
            organization,
            category,
            startDate,
            endDate,
            technologies ?? ["React", "TypeScript"],
            translations ?? [TimelineTranslationInfoBuilder.Build()]
        );
    }
}
