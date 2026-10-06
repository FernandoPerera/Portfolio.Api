using Portfolio.Api.Timeline.Create.Application.Dtos;

namespace Portfolio.Api.Timeline.Update.Application.Dtos;

public record TimelineUpdaterInfo(
    Guid Id,
    string? Organization,
    string? Category,
    string? StartDate,
    string? EndDate,
    List<string>? Technologies,
    List<TimelineTranslationInfo>? Translations
);
