namespace Portfolio.Api.Profiles.Update.Infrastructure.Dtos;

public record UpdateProfileRequest(
    string? Name,
    string? EnterpriseName,
    string? JobRole,
    string? Presentation
);
