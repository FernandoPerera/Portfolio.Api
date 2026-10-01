using Portfolio.Api.Profiles.Find.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Builders;

public static class ProfileFoundBuilder
{
    public static ProfileFound Build(
        string name = "Fernando",
        string? enterprise = "Lean Mind",
        List<ProfileTranslationFound>? translations = null
    )
    {
        return ProfileFound.CreateUnsafe(
                name,
                enterprise,
                translations ?? [ProfileTranslationFoundBuilder.Build()]
            )
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(ProfileFound)} in tests: {error}"));
    }
}
