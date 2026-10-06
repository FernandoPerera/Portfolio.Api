using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Find.Domain.Builders;

public static class TimelineEntryFoundBuilder
{
    public static TimelineEntryFound Build(
        Guid? id = null,
        string organization = "Lean Mind",
        string category = "work",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        List<string>? technologies = null,
        List<TimelineTranslation>? translations = null
    )
    {
        return TimelineEntryFound.CreateUnsafe(
                id ?? Guid.NewGuid(),
                organization,
                category,
                startDate ?? new DateOnly(2023, 1, 1),
                endDate,
                technologies ?? ["C#", ".NET"],
                translations ?? [TimelineTranslationBuilder.Build()]
            )
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(TimelineEntryFound)} in tests: {error}"));
    }
}
