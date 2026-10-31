using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Domain.Models;

public class NameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenName = "Fernando";

        Name.Create(givenName)
            .Match(
                name => name.ToString().ShouldBe(givenName),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenName)
    {
        Name.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Name cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenName = new string('a', 101);

        Name.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The name cannot exceed 100 characters")
            );
    }
}
