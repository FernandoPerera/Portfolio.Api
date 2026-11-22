using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayPointShould
{
    [Theory]
    [InlineData("Claro", "Claro")]
    [InlineData("  Claro  ", "Claro")]
    public void BeCreated(string givenPoint, string expectedPoint)
    {
        WayPoint.Create(givenPoint)
            .Match(
                value => value.ToString().ShouldBe(expectedPoint),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenPoint)
    {
        WayPoint.Create(givenPoint)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Point cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenPoint = new string('a', 120 + 1);

        WayPoint.Create(givenPoint)
            .Match(
                value => value.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The point cannot exceed 120 characters")
            );
    }
}
