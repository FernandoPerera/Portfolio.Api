using System.Security.Cryptography;
using System.Text;
using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class OAuthState
{
    private readonly string _state;

    private OAuthState(string state)
    {
        _state = state;
    }

    public static Either<Error, OAuthState> Create(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            return Error.New(new ValidationException("OAuth state cannot be empty"));
        }

        return new OAuthState(state.Trim());
    }

    public Either<Error, Unit> EnsureMatches(OAuthState expected)
    {
        var matches = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(_state),
            Encoding.UTF8.GetBytes(expected._state)
        );

        return matches ? Unit.Default : Error.New(new UnauthorizedException("Invalid OAuth state"));
    }

    public override string ToString() => _state;
}
