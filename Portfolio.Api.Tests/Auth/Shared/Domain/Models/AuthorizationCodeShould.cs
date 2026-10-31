using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class AuthorizationCodeShould
{
    [Theory]
    [InlineData("value", "value")]
    [InlineData("  value  ", "value")]
    public void BeCreated(string given, string expected)
    {
        AuthorizationCode.Create(given)
            .Match(
                created => created.ToString().ShouldBe(expected),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string given)
    {
        AuthorizationCode.Create(given)
            .Match(
                created => created.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Authorization code cannot be empty")
            );
    }
}
