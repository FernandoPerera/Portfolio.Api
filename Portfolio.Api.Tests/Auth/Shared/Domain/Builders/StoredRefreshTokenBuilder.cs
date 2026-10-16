using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class StoredRefreshTokenBuilder
{
    public static StoredRefreshToken Build(
        DateTimeOffset expiresAt,
        DateTimeOffset? consumedAt = null,
        string tokenHash = "refresh-token-hash",
        string owner = "admin",
        Guid? id = null
    )
    {
        return StoredRefreshToken.CreateUnsafe(id ?? Guid.NewGuid(), tokenHash, owner, expiresAt, consumedAt)
            .IfLeft(error => throw new Exception("Error in StoredRefreshTokenBuilder, error: " + error.Message));
    }
}
