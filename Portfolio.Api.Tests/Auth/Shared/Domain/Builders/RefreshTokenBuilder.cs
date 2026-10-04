using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class RefreshTokenBuilder
{
    public static RefreshToken Build(string refreshToken = "refresh-token")
    {
        return RefreshToken.Create(refreshToken)
            .IfLeft(error => throw new Exception("Error in RefreshTokenBuilder, error: " + error.Message));
    }
}
