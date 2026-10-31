using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AdminUsername
{
    private readonly string _username;

    private AdminUsername(string username)
    {
        _username = username;
    }

    public static Either<DomainError, AdminUsername> Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return new ValidationError("Username cannot be empty");
        }

        return new AdminUsername(username.Trim());
    }

    public override string ToString() => _username;
}
