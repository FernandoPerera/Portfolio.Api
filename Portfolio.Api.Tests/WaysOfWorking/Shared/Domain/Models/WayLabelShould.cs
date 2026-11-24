using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayLabelShould
{
    [Theory]
    [InlineData("Claro", "Claro")]
    [InlineData("  Claro  ", "Claro")]
    public void BeCreated(string givenLabel, string expectedLabel)
    {
        WayLabel.Create(givenLabel)
            .Match(
                value => value.ToString().ShouldBe(expectedLabel),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenLabel)
    {
        WayLabel.Create(givenLabel)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Label cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenLabel = new string('a', 30 + 1);

        WayLabel.Create(givenLabel)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The label cannot exceed 30 characters")
            );
    }
}
