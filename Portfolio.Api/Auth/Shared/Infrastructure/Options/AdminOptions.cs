using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Options;

public record AdminOptions
{
    public const string SectionName = "Admin";

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "The GitHubUserId must be the numeric id of the GitHub user.")]
    public string GitHubUserId { get; init; } = string.Empty;
}
