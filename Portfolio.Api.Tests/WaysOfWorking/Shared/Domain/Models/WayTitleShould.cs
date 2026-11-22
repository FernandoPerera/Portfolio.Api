using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayTitleShould
{
    [Theory]
    [InlineData("Claro", "Claro")]
    [InlineData("  Claro  ", "Claro")]
    public void BeCreated(string givenTitle, string expectedTitle)
    {
        WayTitle.Create(givenTitle)
            .Match(
                value => value.ToString().ShouldBe(expectedTitle),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenTitle)
    {
        WayTitle.Create(givenTitle)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Title cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenTitle = new string('a', 100 + 1);

        WayTitle.Create(givenTitle)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The title cannot exceed 100 characters")
            );
    }
}
