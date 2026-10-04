using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class AdminPasswordShould
{
    [Fact]
    public void BeCreatedKeepingSurroundingSpaces()
    {
        AdminPassword.Create("  secret  ")
            .Match(
                password => password.ToString().ShouldBe("  secret  "),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string given)
    {
        AdminPassword.Create(given)
            .Match(
                password => password.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Password cannot be empty")
            );
    }
}
