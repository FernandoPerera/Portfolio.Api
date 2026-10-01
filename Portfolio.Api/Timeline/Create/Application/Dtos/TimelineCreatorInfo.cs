namespace Portfolio.Api.Timeline.Create.Application.Dtos;

public record TimelineCreatorInfo(
    string Organization,
    string Category,
    string StartDate,
    string? EndDate,
    List<string> Technologies,
    List<TimelineTranslationInfo> Translations
);
