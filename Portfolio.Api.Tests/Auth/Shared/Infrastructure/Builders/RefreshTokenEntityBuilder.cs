using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;

public abstract class RefreshTokenEntityBuilder
{
    public static RefreshTokenEntity Build(
        RefreshToken refreshToken,
        string owner = "admin",
        DateTimeOffset? expiresAt = null,
        DateTimeOffset? consumedAt = null,
        Guid? id = null
    )
    {
        return new RefreshTokenEntity
        {
            Id = id ?? Guid.NewGuid(),
            TokenHash = RefreshTokenHasher.Hash(refreshToken),
            Owner = owner,
            ExpiresAt = expiresAt ?? new DateTimeOffset(2026, 1, 8, 10, 0, 0, TimeSpan.Zero),
            ConsumedAt = consumedAt
        };
    }
}
