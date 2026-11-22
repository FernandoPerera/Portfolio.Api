namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Dtos;

public record UpdateWayTranslationRequest(
    string? Label,
    string? Caption,
    string? Title,
    string? Description,
    List<string>? Points
);
