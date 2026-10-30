namespace Portfolio.Api.Shared.Infrastructure.Options;

public record CorsPolicyOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];
}
