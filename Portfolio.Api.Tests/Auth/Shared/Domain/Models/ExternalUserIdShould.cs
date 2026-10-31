using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class ExternalUserIdShould
{
    [Theory]
    [InlineData("12345", "12345")]
    [InlineData("  12345  ", "12345")]
    public void BeCreated(string given, string expected)
    {
        ExternalUserId.Create(given)
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
        ExternalUserId.Create(given)
            .Match(
                created => created.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("External user id cannot be empty")
            );
    }

    [Fact]
    public void BeCreatedFromANumber()
    {
        ExternalUserId.Create(123456L)
            .Match(
                created => created.ToString().ShouldBe("123456"),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NotAllowANonPositiveNumber(long given)
    {
        ExternalUserId.Create(given)
            .Match(
                created => created.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("External user id must be positive")
            );
    }
}
