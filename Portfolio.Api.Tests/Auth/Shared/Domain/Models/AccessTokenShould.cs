using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class AccessTokenShould
{
    [Theory]
    [InlineData("value", "value")]
    [InlineData("  value  ", "value")]
    public void BeCreated(string given, string expected)
    {
        AccessToken.Create(given)
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
        AccessToken.Create(given)
            .Match(
                created => created.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Access token cannot be empty")
            );
    }
}
