using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Options;

public record AuthOptions
{
    public const string SectionName = "Auth";

    [Required]
    public string FrontendRedirectUri { get; init; } = string.Empty;
}
