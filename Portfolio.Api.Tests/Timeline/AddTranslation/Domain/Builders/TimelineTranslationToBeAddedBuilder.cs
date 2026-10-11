using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.AddTranslation.Domain.Builders;

public abstract class TimelineTranslationToBeAddedBuilder
{
    public static TimelineTranslationToBeAdded Build(
        Guid? entryId = null,
        string languageCode = "en",
        string title = "Frontend Lead",
        string description = "Technical leadership.",
        TimelineEntryFound? current = null
    )
    {
        return TimelineTranslationToBeAdded.Create(
                current ?? TimelineEntryFoundBuilder.Build(
                    entryId,
                    translations: [TimelineTranslationBuilder.Build("es")]
                ),
                languageCode,
                title,
                description
            )
            .IfLeft(error => throw new Exception("Error in TimelineTranslationToBeAddedBuilder, error: " + error.Message));
    }
}
