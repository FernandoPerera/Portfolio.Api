using System.Text.Json.Serialization;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Find.Infrastructure.Dtos;

public record WayTranslationResponse(
    [property: JsonPropertyName("languageCode")] string LanguageCode,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("caption")] string Caption,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("points")] List<string> Points
)
{
    public static WayTranslationResponse From(WayTranslation translation) => new(
        translation.GetLanguageCode(),
        translation.GetLabel(),
        translation.GetCaption(),
        translation.GetTitle(),
        translation.GetDescription(),
        translation.GetPoints()
    );
}
