using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class AuthorizationCodeBuilder
{
    public static AuthorizationCode Build(string code = "github-authorization-code")
    {
        return AuthorizationCode.Create(code)
            .IfLeft(error => throw new Exception("Error in AuthorizationCodeBuilder, error: " + error.Message));
    }
}
