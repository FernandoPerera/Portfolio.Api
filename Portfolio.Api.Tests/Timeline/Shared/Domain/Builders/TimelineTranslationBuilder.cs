using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;

public abstract class TimelineTranslationBuilder
{
    public static TimelineTranslation Build(
        string languageCode = "es",
        string title = "Frontend Lead",
        string description = "Liderazgo técnico en la migración a arquitecturas Jamstack."
    )
    {
        return TimelineTranslation.Create(languageCode, title, description)
            .IfLeft(error => throw new Exception("Error in TimelineTranslationBuilder, error: " + error.Message));
    }
}
