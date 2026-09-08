namespace Portfolio.Api.Profile.Create.Infrastructure.Dtos;

public record CreateProfileRequest(string Name, string Role, string Presentation, string Enterprise);