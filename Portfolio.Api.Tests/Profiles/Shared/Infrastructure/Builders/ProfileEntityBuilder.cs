using Portfolio.Api.Profiles.Create.Infrastructure.Entities;

namespace Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;

public static class ProfileEntityBuilder
{
    public static ProfileEntity Build(
        Guid id,
        string name = "Fernando",
        string? enterprise = "Lean Mind",
        List<ProfileTranslationEntity>? translations = null
    )
    {
        return new ProfileEntity
        {
            Id = id,
            Name = name,
            Enterprise = enterprise ?? string.Empty,
            Translations = translations ?? []
        };
    }
}
