using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Builders;

public abstract class TimelineEntryToBeCreatedBuilder
{
    public static TimelineEntryToBeCreated Build(
        string organization = "Lean Mind",
        string category = "work",
        string startDate = "2023-01",
        string? endDate = null,
        List<string>? technologies = null,
        List<TimelineTranslation>? translations = null
    )
    {
        return TimelineEntryToBeCreated.Create(
            organization,
            category,
            startDate,
            endDate,
            technologies ?? ["React", "TypeScript"],
            translations ?? [TimelineTranslationBuilder.Build()]
        ).IfLeft(error => throw new Exception("Error in TimelineEntryToBeCreatedBuilder, error: " + error.Message));
    }
}
