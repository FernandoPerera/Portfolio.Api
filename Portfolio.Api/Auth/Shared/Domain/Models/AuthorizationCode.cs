using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AuthorizationCode
{
    private readonly string _code;

    private AuthorizationCode(string code)
    {
        _code = code;
    }

    public static Either<DomainError, AuthorizationCode> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new ValidationError("Authorization code cannot be empty");
        }

        return new AuthorizationCode(code.Trim());
    }

    public override string ToString() => _code;
}
