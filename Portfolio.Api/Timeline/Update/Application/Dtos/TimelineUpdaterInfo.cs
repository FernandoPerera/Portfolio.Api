namespace Portfolio.Api.Timeline.Update.Application.Dtos;

public record TimelineUpdaterInfo(
    Guid Id,
    string? Organization,
    string? Category,
    string? StartDate,
    string? EndDate,
    List<string>? Technologies,
    string? Title,
    string? Description,
    string LanguageCode
);
