using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class PositionShould
{
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    public void BeCreated(int givenPosition)
    {
        Position.Create(givenPosition)
            .Match(
                position => position.ToInt().ShouldBe(givenPosition),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectPositionsBelowOne(int givenPosition)
    {
        Position.Create(givenPosition)
            .Match(
                position => position.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Position must be 1 or greater")
            );
    }
}
