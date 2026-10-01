using Portfolio.Api.Profiles.Create.Infrastructure.Entities;

namespace Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;

public static class ProfileTranslationEntityBuilder
{
    public static ProfileTranslationEntity Build(
        Guid profileId,
        string languageCode = "es",
        string presentation = "Mucho gusto :)",
        string role = "Beer software architect"
    )
    {
        return new ProfileTranslationEntity
        {
            ProfileId = profileId,
            LanguageCode = languageCode,
            Presentation = presentation,
            Role = role
        };
    }
}
