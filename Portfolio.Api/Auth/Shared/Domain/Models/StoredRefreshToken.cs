using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class StoredRefreshToken
{
    private readonly RefreshTokenId _id;
    private readonly RefreshTokenHash _tokenHash;
    private readonly AdminUsername _owner;
    private readonly ExpiresAt _expiresAt;
    private readonly ConsumedAt _consumedAt;

    private StoredRefreshToken(
        RefreshTokenId id,
        RefreshTokenHash tokenHash,
        AdminUsername owner,
        ExpiresAt expiresAt,
        ConsumedAt consumedAt
    )
    {
        _id = id;
        _tokenHash = tokenHash;
        _owner = owner;
        _expiresAt = expiresAt;
        _consumedAt = consumedAt;
    }

    public static Either<Error, StoredRefreshToken> Create(
        Guid id,
        string tokenHash,
        string owner,
        DateTimeOffset expiresAt
    ) =>
        from refreshTokenId in RefreshTokenId.Create(id)
        from hash in RefreshTokenHash.Create(tokenHash)
        from adminUsername in AdminUsername.Create(owner)
        from expiration in ExpiresAt.Create(expiresAt)
        from consumption in ConsumedAt.NotConsumed()
        select new StoredRefreshToken(refreshTokenId, hash, adminUsername, expiration, consumption);

    public static Either<Error, StoredRefreshToken> CreateUnsafe(
        Guid id,
        string tokenHash,
        string owner,
        DateTimeOffset expiresAt,
        DateTimeOffset? consumedAt
    ) =>
        from refreshTokenId in RefreshTokenId.CreateUnsafe(id)
        from hash in RefreshTokenHash.CreateUnsafe(tokenHash)
        from adminUsername in AdminUsername.Create(owner)
        from expiration in ExpiresAt.CreateUnsafe(expiresAt)
        from consumption in ConsumedAt.CreateUnsafe(consumedAt)
        select new StoredRefreshToken(refreshTokenId, hash, adminUsername, expiration, consumption);

    public Either<Error, StoredRefreshToken> Consume(DateTimeOffset now) =>
        from consumedAt in ConsumedAt.Create(now)
        from consumed in Consume(consumedAt)
        select consumed;

    private Either<Error, StoredRefreshToken> Consume(ConsumedAt consumedAt)
    {
        if (_consumedAt.IsConsumed())
        {
            return Error.New(new RefreshTokenReuseException("Refresh token has already been used"));
        }

        if (_expiresAt.HasExpired(consumedAt))
        {
            return Error.New(new UnauthorizedException("Invalid refresh token"));
        }

        return new StoredRefreshToken(_id, _tokenHash, _owner, _expiresAt, consumedAt);
    }

    public Guid GetId() => _id.GetValue();

    public string GetTokenHash() => _tokenHash.ToString();

    public string GetOwner() => _owner.ToString();

    public AdminUsername GetOwnerUsername() => _owner;

    public DateTimeOffset GetExpiresAt() => _expiresAt.GetValue();

    public DateTimeOffset? GetConsumedAt() => _consumedAt.GetValue();
}
