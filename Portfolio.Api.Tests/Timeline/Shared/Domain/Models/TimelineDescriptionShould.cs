using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TimelineDescriptionShould
{
    [Theory]
    [InlineData("Liderazgo técnico en la migración a Jamstack.", "Liderazgo técnico en la migración a Jamstack.")]
    [InlineData("  Liderazgo técnico  ", "Liderazgo técnico")]
    public void BeCreated(string givenDescription, string expectedDescription)
    {
        TimelineDescription.Create(givenDescription)
            .Match(
                description => description.ToString().ShouldBe(expectedDescription),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenDescription)
    {
        TimelineDescription.Create(givenDescription)
            .Match(
                description => description.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Description cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenDescription = new string('a', 1001);

        TimelineDescription.Create(givenDescription)
            .Match(
                description => description.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The description cannot exceed 1000 characters")
            );
    }
}
