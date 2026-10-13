using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class LoginCodeBuilder
{
    public static LoginCode Build(string loginCode = "login-code")
    {
        return LoginCode.Create(loginCode)
            .IfLeft(error => throw new Exception("Error in LoginCodeBuilder, error: " + error.Message));
    }
}
