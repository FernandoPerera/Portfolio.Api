namespace Portfolio.Api.WaysOfWorking.Create.Infrastructure.Dtos;

public record CreateWayOfWorkingRequest(
    string Illustration,
    List<WayTranslationRequest> Translations
);

public record WayTranslationRequest(
    string LanguageCode,
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
