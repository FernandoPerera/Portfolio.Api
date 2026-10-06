using Portfolio.Api.Profiles.Update.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Update.Domain.Builders;

public abstract class ProfileToBeUpdatedBuilder
{
    public static ProfileToBeUpdated Build(
        string? name = "Fernando",
        string? enterprise = "Lean Mind",
        string? jobRole = "Beer software architect",
        string? presentation = "Mucho gusto :)",
        string languageCode = "es"
    )
    {
        return ProfileToBeUpdated.Create(
            name,
            enterprise,
            jobRole,
            presentation,
            languageCode
        ).IfLeft(error => throw new Exception("Error in ProfileToBeUpdatedBuilder, error: " + error.Message));
    }
}
