using System.Text.Json.Serialization;
using Portfolio.Api.Timeline.Find.Domain.Models;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Dtos;

public record TimelineEntryResponse(
    [property: JsonPropertyName("organization")] string Organization,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("startDate")] string StartDate,
    [property: JsonPropertyName("endDate")] string? EndDate,
    [property: JsonPropertyName("technologies")] List<string> Technologies,
    [property: JsonPropertyName("translations")] List<TimelineTranslationResponse> Translations
)
{
    public static TimelineEntryResponse From(TimelineEntryFound entry) => new(
        entry.GetOrganization(),
        entry.GetCategory(),
        entry.GetStartDate(),
        entry.GetEndDate(),
        entry.GetTechnologies(),
        entry.GetTranslations().Select(TimelineTranslationResponse.From).ToList()
    );
}
