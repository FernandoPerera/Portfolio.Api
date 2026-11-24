namespace Portfolio.Api.WaysOfWorking.Create.Application.Dtos;

public record WayTranslationInfo(
    string LanguageCode,
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
