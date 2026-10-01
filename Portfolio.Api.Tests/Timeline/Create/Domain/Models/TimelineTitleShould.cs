using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Models;

public class TimelineTitleShould
{
    [Theory]
    [InlineData("Frontend Lead", "Frontend Lead")]
    [InlineData("  Frontend Lead  ", "Frontend Lead")]
    public void BeCreated(string givenTitle, string expectedTitle)
    {
        TimelineTitle.Create(givenTitle)
            .Match(
                title => title.ToString().ShouldBe(expectedTitle),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenTitle)
    {
        TimelineTitle.Create(givenTitle)
            .Match(
                title => title.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Title cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenTitle = new string('a', 101);

        TimelineTitle.Create(givenTitle)
            .Match(
                title => title.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("The title cannot exceed 100 characters")
            );
    }
}
