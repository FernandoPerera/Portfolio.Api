using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class StoredRefreshTokenBuilder
{
    public static StoredRefreshToken Build(
        DateTimeOffset expiresAt,
        DateTimeOffset? consumedAt = null,
        string tokenHash = "refresh-token-hash",
        string owner = "admin"
    )
    {
        return StoredRefreshToken.CreateUnsafe(Guid.NewGuid(), tokenHash, owner, expiresAt, consumedAt)
            .IfLeft(error => throw new Exception("Error in StoredRefreshTokenBuilder, error: " + error.Message));
    }
}
