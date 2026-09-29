using Portfolio.Api.Profiles.Find.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Builders;

public static class ProfileFoundBuilder
{
    public static ProfileFound Build(
        string name = "Fernando",
        string jobRole = "Beer software architect",
        string presentation = "Mucho gusto :)",
        string? enterprise = "Lean Mind"
    )
    {
        return ProfileFound.CreateUnsafe(name, presentation, jobRole, enterprise)
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(ProfileFound)} in tests: {error}"));
    }
}
