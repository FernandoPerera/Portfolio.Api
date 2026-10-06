using System.Text.Json.Serialization;
using Portfolio.Api.Languages.Find.Domain.Models;

namespace Portfolio.Api.Languages.Find.Infrastructure.Dtos;

public record LanguageResponse(
    [property: JsonPropertyName("languageCode")] string LanguageCode,
    [property: JsonPropertyName("name")] string Name
)
{
    public static LanguageResponse From(LanguageFound language) => new(
        language.GetLanguageCode(),
        language.GetName()
    );
}
