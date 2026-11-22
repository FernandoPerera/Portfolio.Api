using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayPointsShould
{
    [Fact]
    public void BeCreated()
    {
        var givenPoints = new List<string> { "Una charla corta", "Un resumen escrito" };

        WayPoints.Create(givenPoints)
            .Match(
                points => points.GetValues().ShouldBe(givenPoints),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void RejectPointsOutsideTheAllowedAmount(int amount)
    {
        var givenPoints = Enumerable.Range(1, amount).Select(number => $"Point {number}").ToList();

        WayPoints.Create(givenPoints)
            .Match(
                points => points.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("There must be between 1 and 5 points")
            );
    }

    [Theory]
    [InlineData("Una charla", "Una charla")]
    [InlineData("Una charla", "una charla")]
    [InlineData("Una charla", "  UNA CHARLA ")]
    public void RejectPointsWhenOneIsRepeated(string first, string second)
    {
        WayPoints.Create([first, second])
            .Match(
                points => points.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Points cannot be repeated")
            );
    }
}
