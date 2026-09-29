using Portfolio.Api.Profiles.Create.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Builders;

public abstract class ProfileToBeCreatedBuilder
{
    public static ProfileToBeCreated Build(
        string name = "Fernando",
        string presentation = "Mucho gusto :)",
        string jobRole = "Beer software architect",
        string? enterprise = "Lean Mind",
        string languageCode = "es"
    )
    {
        return ProfileToBeCreated.Create(
            name,
            presentation,
            jobRole,
            enterprise,
            languageCode
        ).IfLeft(error => throw new Exception("Error in ProfileToBeCreatedBuilder, error: " + error.Message));
    }
}