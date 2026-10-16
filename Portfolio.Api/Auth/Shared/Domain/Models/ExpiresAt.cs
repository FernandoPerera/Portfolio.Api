using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class ExpiresAt
{
    private readonly DateTimeOffset _expiresAt;

    private ExpiresAt(DateTimeOffset expiresAt)
    {
        _expiresAt = expiresAt;
    }

    public static Either<Error, ExpiresAt> Create(DateTimeOffset expiresAt) => new ExpiresAt(expiresAt);

    public static Either<Error, ExpiresAt> CreateUnsafe(DateTimeOffset expiresAt) => new ExpiresAt(expiresAt);

    public bool HasExpired(ConsumedAt consumedAt) => consumedAt.IsAtOrAfter(_expiresAt);

    public DateTimeOffset GetValue() => _expiresAt;
}
