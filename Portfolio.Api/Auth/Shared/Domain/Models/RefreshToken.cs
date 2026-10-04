using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class RefreshToken
{
    private readonly string _refreshToken;

    private RefreshToken(string refreshToken)
    {
        _refreshToken = refreshToken;
    }

    public static Either<Error, RefreshToken> Create(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Error.New(new ValidationException("Refresh token cannot be empty"));
        }

        return new RefreshToken(refreshToken.Trim());
    }

    public override string ToString() => _refreshToken;
}
