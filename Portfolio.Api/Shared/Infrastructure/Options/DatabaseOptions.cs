using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Shared.Infrastructure.Options;

public record DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}
