using System.Text.Json.Serialization;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Find.Infrastructure.Dtos;

public record WayOfWorkingResponse(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("position")] int Position,
    [property: JsonPropertyName("illustration")] string Illustration,
    [property: JsonPropertyName("translations")] List<WayTranslationResponse> Translations
)
{
    public static WayOfWorkingResponse From(WayOfWorkingFound way) => new(
        way.GetId(),
        way.GetPosition(),
        way.GetIllustration(),
        way.GetTranslations().Select(WayTranslationResponse.From).ToList()
    );
}
