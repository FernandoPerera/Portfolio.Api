using Portfolio.Api.Profiles.Update.Infrastructure.Dtos;

namespace Portfolio.Api.Tests.Profiles.Update.Infrastructure.Builders;

public abstract class UpdateProfileRequestBuilder
{
    public static UpdateProfileRequest Build(
        string? name = "Fernando",
        string? enterpriseName = "Lean Mind",
        string? jobRole = "Beer software architect",
        string? presentation = "Mucho gusto :)"
    )
    {
        return new UpdateProfileRequest(name, enterpriseName, jobRole, presentation);
    }
}
