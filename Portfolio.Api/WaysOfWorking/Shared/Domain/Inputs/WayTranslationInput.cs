namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

public record WayTranslationInput(
    string LanguageCode,
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
