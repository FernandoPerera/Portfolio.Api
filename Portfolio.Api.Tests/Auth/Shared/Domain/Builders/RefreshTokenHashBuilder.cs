using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class RefreshTokenHashBuilder
{
    public static RefreshTokenHash Build(string hash = "refresh-token-hash")
    {
        return RefreshTokenHash.Create(hash)
            .IfLeft(error => throw new Exception("Error in RefreshTokenHashBuilder, error: " + error.Message));
    }
}
