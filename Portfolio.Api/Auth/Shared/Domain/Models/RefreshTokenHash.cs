using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class RefreshTokenHash
{
    private readonly string _hash;

    private RefreshTokenHash(string hash)
    {
        _hash = hash;
    }

    public static Either<DomainError, RefreshTokenHash> Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return new ValidationError("Refresh token hash cannot be empty");
        }

        return new RefreshTokenHash(hash.Trim());
    }

    public static Either<DomainError, RefreshTokenHash> CreateUnsafe(string hash) => new RefreshTokenHash(hash);

    public override string ToString() => _hash;
}
