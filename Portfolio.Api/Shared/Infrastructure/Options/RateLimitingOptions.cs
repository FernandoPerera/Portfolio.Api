using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Portfolio.Api.Shared.Infrastructure.Options;

public record RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    [ValidateObjectMembers]
    public RateLimitPolicyOptions Auth { get; init; } = new() { PermitLimit = 10 };

    [ValidateObjectMembers]
    public RateLimitPolicyOptions Public { get; init; } = new() { PermitLimit = 100 };
}

public record RateLimitPolicyOptions
{
    [Range(1, int.MaxValue)]
    public int PermitLimit { get; init; }

    [Range(1, int.MaxValue)]
    public int WindowSeconds { get; init; } = 60;
}
