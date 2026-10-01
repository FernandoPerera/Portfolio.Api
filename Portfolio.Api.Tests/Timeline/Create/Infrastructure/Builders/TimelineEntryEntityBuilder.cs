using Portfolio.Api.Timeline.Create.Infrastructure.Entities;

namespace Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;

public static class TimelineEntryEntityBuilder
{
    public static TimelineEntryEntity Build(
        Guid entryId,
        string organization = "Lean Mind",
        string category = "work",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        List<string>? technologies = null,
        List<TimelineEntryTranslationEntity>? translations = null
    )
    {
        var entry = new TimelineEntryEntity
        {
            Id = entryId,
            Organization = organization,
            Category = category,
            StartDate = startDate ?? new DateOnly(2023, 1, 1),
            EndDate = endDate,
            Technologies = technologies ?? ["React", "TypeScript"],
            Translations = translations ?? [TimelineEntryTranslationEntityBuilder.Build(entryId)]
        };

        foreach (var translation in entry.Translations)
        {
            translation.Entry = entry;
        }

        return entry;
    }
}
