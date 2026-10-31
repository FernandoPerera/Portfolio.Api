using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class LoginCode
{
    private readonly string _loginCode;

    private LoginCode(string loginCode)
    {
        _loginCode = loginCode;
    }

    public static Either<DomainError, LoginCode> Create(string loginCode)
    {
        if (string.IsNullOrWhiteSpace(loginCode))
        {
            return new ValidationError("Login code cannot be empty");
        }

        return new LoginCode(loginCode.Trim());
    }

    public override string ToString() => _loginCode;
}
