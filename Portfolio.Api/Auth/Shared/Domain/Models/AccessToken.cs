using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AccessToken
{
    private readonly string _accessToken;

    private AccessToken(string accessToken)
    {
        _accessToken = accessToken;
    }

    public static Either<Error, AccessToken> Create(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Error.New(new ValidationException("Access token cannot be empty"));
        }

        return new AccessToken(accessToken.Trim());
    }

    public override string ToString() => _accessToken;
}
