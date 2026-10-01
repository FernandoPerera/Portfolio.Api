using System.Text.Json.Serialization;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Dtos;

public record ProfileResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("enterprise")] string Enterprise,
    [property: JsonPropertyName("translations")] List<ProfileTranslationResponse> Translations
);

public record ProfileTranslationResponse(
    [property: JsonPropertyName("languageCode")] string LanguageCode,
    [property: JsonPropertyName("presentation")] string Presentation,
    [property: JsonPropertyName("jobRole")] string JobRole
);
