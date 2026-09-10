using Portfolio.Api.Profiles.Create.Domain.Models;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Builders;

public abstract class ProfileToBeCreatedBuilder
{
    public static ProfileToBeCreated Build()
    {
        return ProfileToBeCreated.Create(
            "Fernando",
            "Mucho gusto :)",
            "Beer software architect",
            "Lean Mind"
        ).IfLeft(error => throw new Exception("Error in ProfileToBeCreatedBuilder, error: " + error.Message));
    }
}