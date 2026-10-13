using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Options;

public record GitHubOptions
{
    public const string SectionName = "GitHub";

    [Required]
    public string ClientId { get; init; } = string.Empty;

    [Required]
    public string ClientSecret { get; init; } = string.Empty;

    [Required]
    public string RedirectUri { get; init; } = string.Empty;

    [Required]
    public string AuthorizeEndpoint { get; init; } = string.Empty;

    [Required]
    public string TokenEndpoint { get; init; } = string.Empty;

    [Required]
    public string UserEndpoint { get; init; } = string.Empty;

    [Required]
    public string UserAgent { get; init; } = string.Empty;
}
