using Portfolio.Api.Timeline.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Update.Infrastructure.Dtos;

public record UpdateTimelineRequest(
    string? Organization,
    string? Category,
    string? StartDate,
    string? EndDate,
    List<string>? Technologies,
    List<TimelineTranslationRequest>? Translations
);
