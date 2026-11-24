namespace Portfolio.Api.WaysOfWorking.Add.Infrastructure.Dtos;

public record AddWayRequest(
    string Label,
    string Caption,
    string Title,
    string Description,
    List<string> Points
);
