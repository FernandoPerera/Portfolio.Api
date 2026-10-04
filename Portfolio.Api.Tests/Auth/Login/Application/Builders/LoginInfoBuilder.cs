using Portfolio.Api.Auth.Login.Application.Dtos;

namespace Portfolio.Api.Tests.Auth.Login.Application.Builders;

public abstract class LoginInfoBuilder
{
    public static LoginInfo Build(string username = "admin", string password = "secret")
    {
        return new LoginInfo(username, password);
    }
}
