using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class AdminCredentials
{
    private readonly AdminUsername _username;
    private readonly AdminPassword _password;

    private AdminCredentials(AdminUsername username, AdminPassword password)
    {
        _username = username;
        _password = password;
    }

    public static Either<Error, AdminCredentials> Create(string givenUsername, string givenPassword) =>
        from username in AdminUsername.Create(givenUsername)
        from password in AdminPassword.Create(givenPassword)
        select new AdminCredentials(username, password);

    public AdminUsername GetUsername() => _username;

    public string GetPassword() => _password.ToString();
}
