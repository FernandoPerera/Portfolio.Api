using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class LoginCode
{
    private readonly string _loginCode;

    private LoginCode(string loginCode)
    {
        _loginCode = loginCode;
    }

    public static Either<Error, LoginCode> Create(string loginCode)
    {
        if (string.IsNullOrWhiteSpace(loginCode))
        {
            return Error.New(new ValidationException("Login code cannot be empty"));
        }

        return new LoginCode(loginCode.Trim());
    }

    public override string ToString() => _loginCode;
}
