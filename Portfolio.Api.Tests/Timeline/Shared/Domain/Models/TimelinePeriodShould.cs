using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TimelinePeriodShould
{
    [Fact]
    public void BeCreated()
    {
        TimelinePeriod.Create("2021-09", "2023-06")
            .Match(
                period =>
                {
                    period.GetStartDate().ToDateOnly().ShouldBe(new DateOnly(2021, 9, 1));
                    period.GetEndDate().ToDateOnly().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1)));
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void AllowStartAndEndInTheSameMonth()
    {
        TimelinePeriod.Create("2023-01", "2023-01")
            .Match(
                period => period.GetEndDate().ToDateOnly().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 1, 1))),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectPeriodWhenEndIsBeforeStart()
    {
        TimelinePeriod.Create("2023-01", "2022-12")
            .Match(
                period => period.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The end date cannot be earlier than the start date")
            );
    }
}
