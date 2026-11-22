using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayCaptionShould
{
    [Theory]
    [InlineData("Claro", "Claro")]
    [InlineData("  Claro  ", "Claro")]
    public void BeCreated(string givenCaption, string expectedCaption)
    {
        WayCaption.Create(givenCaption)
            .Match(
                value => value.ToString().ShouldBe(expectedCaption),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenCaption)
    {
        WayCaption.Create(givenCaption)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Caption cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenCaption = new string('a', 60 + 1);

        WayCaption.Create(givenCaption)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The caption cannot exceed 60 characters")
            );
    }
}
