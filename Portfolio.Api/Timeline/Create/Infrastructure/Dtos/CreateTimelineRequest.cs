namespace Portfolio.Api.Timeline.Create.Infrastructure.Dtos;

public record CreateTimelineRequest(
    string Organization,
    string Category,
    string StartDate,
    string? EndDate,
    List<string>? Technologies,
    List<TimelineTranslationRequest> Translations
);

public record TimelineTranslationRequest(
    string LanguageCode,
    string Title,
    string Description
);
