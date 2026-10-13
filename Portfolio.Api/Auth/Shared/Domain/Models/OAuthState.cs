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

    public override string ToString() => _state;
}
