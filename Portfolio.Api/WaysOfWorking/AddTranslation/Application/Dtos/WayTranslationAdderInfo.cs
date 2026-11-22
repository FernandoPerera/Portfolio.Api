namespace Portfolio.Api.WaysOfWorking.AddTranslation.Application.Dtos;

public record WayTranslationAdderInfo(
    Guid Id,
    string LanguageCode,
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
