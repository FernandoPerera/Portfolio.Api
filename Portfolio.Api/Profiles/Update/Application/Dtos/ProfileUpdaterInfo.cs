namespace Portfolio.Api.Profiles.Update.Application.Dtos;

public record ProfileUpdaterInfo(
    string? Name,
    string? EnterpriseName,
    string? JobRole,
    string? Presentation,
    string LanguageCode
);
