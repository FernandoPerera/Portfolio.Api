using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Tests.Profiles.Create.Infrastructure.Builders;

public abstract class CreateProfileRequestBuilder
{
    public static CreateProfileRequest Build(
        string name = "Fernando",
        string jobRole = "Beer software architect",
        string presentation = "Mucho gusto :)",
        string? enterpriseName = "Lean Mind",
        string languageCode = "es"
    )
    {
        return new CreateProfileRequest(name, jobRole, presentation, enterpriseName, languageCode);
    }
}
