using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;

namespace Portfolio.Api.Tests.Profiles.Update.Domain.Builders;

public abstract class ProfileToBeUpdatedBuilder
{
    public static ProfileToBeUpdated Build(
        string? name = "Fernando",
        string? enterprise = "Lean Mind",
        string? jobRole = "Beer software architect",
        string? presentation = "Mucho gusto :)",
        string languageCode = "es",
        ProfileFound? current = null
    )
    {
        return ProfileToBeUpdated.Create(
            current ?? ProfileFoundBuilder.Build(translations: [ProfileTranslationFoundBuilder.Build(languageCode)]),
            name,
            enterprise,
            jobRole,
            presentation,
            languageCode
        ).IfLeft(error => throw new Exception("Error in ProfileToBeUpdatedBuilder, error: " + error.Message));
    }
}
