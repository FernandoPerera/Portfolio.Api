using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AdminPassword
{
    private readonly string _password;

    private AdminPassword(string password)
    {
        _password = password;
    }

    public static Either<Error, AdminPassword> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Error.New(new ValidationException("Password cannot be empty"));
        }

        return new AdminPassword(password);
    }

    public override string ToString() => _password;
}
