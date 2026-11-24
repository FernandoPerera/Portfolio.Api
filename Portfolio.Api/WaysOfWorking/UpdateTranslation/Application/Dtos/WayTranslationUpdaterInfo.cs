namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.Dtos;

public record WayTranslationUpdaterInfo(
    Guid Id,
    string LanguageCode,
    string? Label,
    string? Caption,
    string? Title,
    string? Description,
    List<string>? Points
);
