using Portfolio.Api.Timeline.Create.Application.Dtos;

namespace Portfolio.Api.Tests.Timeline.Create.Application.Builders;

public abstract class TimelineTranslationInfoBuilder
{
    public static TimelineTranslationInfo Build(
        string languageCode = "es",
        string title = "Frontend Lead",
        string description = "Liderazgo técnico en la migración a arquitecturas Jamstack."
    )
    {
        return new TimelineTranslationInfo(languageCode, title, description);
    }
}
