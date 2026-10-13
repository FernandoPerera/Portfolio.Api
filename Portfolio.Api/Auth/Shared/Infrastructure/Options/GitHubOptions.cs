using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Options;

public record GitHubOptions
{
    public const string SectionName = "GitHub";

    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string RedirectUri { get; init; } = string.Empty;

    public string AuthorizeEndpoint { get; init; } = string.Empty;

    public string TokenEndpoint { get; init; } = string.Empty;

    public string UserEndpoint { get; init; } = string.Empty;

    public string UserAgent { get; init; } = string.Empty;
}
