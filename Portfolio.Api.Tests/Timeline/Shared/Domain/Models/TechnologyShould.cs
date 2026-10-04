using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TechnologyShould
{
    [Theory]
    [InlineData("Next.js", "Next.js")]
    [InlineData("  React  ", "React")]
    public void BeCreated(string givenTechnology, string expectedTechnology)
    {
        Technology.Create(givenTechnology)
            .Match(
                technology => technology.ToString().ShouldBe(expectedTechnology),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenTechnology)
    {
        Technology.Create(givenTechnology)
            .Match(
                technology => technology.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Technology cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenTechnology = new string('a', 51);

        Technology.Create(givenTechnology)
            .Match(
                technology => technology.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("The technology cannot exceed 50 characters")
            );
    }
}
