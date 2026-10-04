using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class AccessTokenBuilder
{
    public static AccessToken Build(string accessToken = "access-token")
    {
        return AccessToken.Create(accessToken)
            .IfLeft(error => throw new Exception("Error in AccessTokenBuilder, error: " + error.Message));
    }
}
