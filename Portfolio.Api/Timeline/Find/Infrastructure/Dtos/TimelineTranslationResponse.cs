using System.Text.Json.Serialization;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Dtos;

public record TimelineTranslationResponse(
    [property: JsonPropertyName("languageCode")] string LanguageCode,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description
)
{
    public static TimelineTranslationResponse From(TimelineTranslation translation) => new(
        translation.GetLanguageCode(),
        translation.GetTitle(),
        translation.GetDescription()
    );
}
