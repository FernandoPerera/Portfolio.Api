using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class OAuthStateBuilder
{
    public static OAuthState Build(string state = "oauth-state")
    {
        return OAuthState.Create(state)
            .IfLeft(error => throw new Exception("Error in OAuthStateBuilder, error: " + error.Message));
    }
}
