using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using LanguageExt;
using Portfolio.Api.Tests.Timeline.Create.Domain.Builders;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class EndDateShould
{
    [Fact]
    public void BeCreated()
    {
        EndDate.Create("2023-06")
            .Match(
                endDate => endDate.ToDateOnly().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1))),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeEmptyWhenThereIsNoEndDate()
    {
        EndDate.Create(null)
            .Match(
                endDate => endDate.ToDateOnly().ShouldBe(Option<DateOnly>.None),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("2022-12", "2023-01", true)]
    [InlineData("2023-01", "2023-01", false)]
    [InlineData("2023-02", "2023-01", false)]
    [InlineData(null, "2023-01", false)]
    public void KnowIfItIsBeforeTheStartDate(string? givenEndDate, string givenStartDate, bool expected)
    {
        var endDate = EndDateBuilder.Build(givenEndDate);
        var startDate = StartDateBuilder.Build(givenStartDate);

        endDate.IsBefore(startDate).ShouldBe(expected);
    }
}
