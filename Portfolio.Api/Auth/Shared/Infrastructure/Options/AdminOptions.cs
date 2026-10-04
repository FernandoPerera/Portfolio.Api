namespace Portfolio.Api.Auth.Shared.Infrastructure.Options;

public record AdminOptions
{
    public const string SectionName = "Admin";

    public string Username { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;
}
