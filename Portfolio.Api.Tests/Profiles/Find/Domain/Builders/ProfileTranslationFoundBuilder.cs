using Portfolio.Api.Profiles.Find.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Builders;

public static class ProfileTranslationFoundBuilder
{
    public static ProfileTranslationFound Build(
        string languageCode = "es",
        string presentation = "Mucho gusto :)",
        string jobRole = "Beer software architect"
    )
    {
        return ProfileTranslationFound.CreateUnsafe(languageCode, presentation, jobRole)
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(ProfileTranslationFound)} in tests: {error}"));
    }
}
