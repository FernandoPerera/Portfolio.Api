namespace Portfolio.Api.WaysOfWorking.Add.Application.Dtos;

public record WayAdderInfo(
    Guid Id,
    string LanguageCode,
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
