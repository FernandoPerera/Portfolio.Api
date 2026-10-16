using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class RefreshTokenHash
{
    private readonly string _hash;

    private RefreshTokenHash(string hash)
    {
        _hash = hash;
    }

    public static Either<Error, RefreshTokenHash> Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return Error.New(new ValidationException("Refresh token hash cannot be empty"));
        }

        return new RefreshTokenHash(hash.Trim());
    }

    public static Either<Error, RefreshTokenHash> CreateUnsafe(string hash) => new RefreshTokenHash(hash);

    public override string ToString() => _hash;
}
