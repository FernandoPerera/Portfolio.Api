namespace Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Dtos;

public record AddWayTranslationRequest(
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
