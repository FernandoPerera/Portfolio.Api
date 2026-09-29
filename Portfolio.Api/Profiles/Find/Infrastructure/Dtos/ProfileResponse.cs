using System.Text.Json.Serialization;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Dtos;

public record ProfileResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("Presentation")] string Presentation,
    [property: JsonPropertyName("jobRole")] string JobRole,
    [property: JsonPropertyName("Enterprise")] string Enterprise
);
