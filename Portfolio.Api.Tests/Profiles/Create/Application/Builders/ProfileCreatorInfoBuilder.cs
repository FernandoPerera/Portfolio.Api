using Portfolio.Api.Profiles.Create.Application.Dtos;

namespace Portfolio.Api.Tests.Profiles.Create.Application.Builders;

public abstract class ProfileCreatorInfoBuilder
{
    public static ProfileCreatorInfo Build(
        string name = "Fernando",
        string jobRole = "Beer software architect",
        string presentation = "Mucho gusto :)",
        string? enterpriseName = "Lean Mind",
        string languageCode = "es"
    )
    {
        return new ProfileCreatorInfo(name, jobRole, presentation, enterpriseName, languageCode);
    }
}
