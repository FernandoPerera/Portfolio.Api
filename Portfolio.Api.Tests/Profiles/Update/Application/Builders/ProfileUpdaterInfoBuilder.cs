using Portfolio.Api.Profiles.Update.Application.Dtos;

namespace Portfolio.Api.Tests.Profiles.Update.Application.Builders;

public abstract class ProfileUpdaterInfoBuilder
{
    public static ProfileUpdaterInfo Build(
        string? name = "Fernando",
        string? enterpriseName = "Lean Mind",
        string? jobRole = "Beer software architect",
        string? presentation = "Mucho gusto :)",
        string languageCode = "es"
    )
    {
        return new ProfileUpdaterInfo(name, enterpriseName, jobRole, presentation, languageCode);
    }
}
