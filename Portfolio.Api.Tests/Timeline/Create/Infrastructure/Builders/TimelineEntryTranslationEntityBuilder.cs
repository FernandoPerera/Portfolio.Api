using Portfolio.Api.Timeline.Create.Infrastructure.Entities;

namespace Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;

public static class TimelineEntryTranslationEntityBuilder
{
    public static TimelineEntryTranslationEntity Build(
        Guid entryId,
        string languageCode = "es",
        string title = "Frontend Lead",
        string description = "Liderazgo técnico en la migración a arquitecturas Jamstack."
    )
    {
        return new TimelineEntryTranslationEntity
        {
            EntryId = entryId,
            LanguageCode = languageCode,
            Title = title,
            Description = description
        };
    }
}
