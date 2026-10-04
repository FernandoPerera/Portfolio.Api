using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class StartDateShould
{
    [Fact]
    public void BeCreated()
    {
        StartDate.Create("2021-09")
            .Match(
                startDate => startDate.ToDateOnly().ShouldBe(new DateOnly(2021, 9, 1)),
                error => error.ShouldBeNull()
            );
    }
}
