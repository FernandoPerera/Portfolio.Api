namespace Portfolio.Api.Profiles.Create.Infrastructure.Dtos;

public record CreateProfileRequest(string Name, string JobRole, string Presentation, string? EnterpriseName);