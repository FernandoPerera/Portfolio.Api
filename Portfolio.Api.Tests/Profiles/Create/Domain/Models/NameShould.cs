using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Domain.Models;

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
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Name cannot be empty")
            );
    }
    
}