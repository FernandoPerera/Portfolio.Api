using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class YearMonthShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenDate = "2023-01";

        YearMonth.Create(givenDate)
            .Match(
                yearMonth => yearMonth.ToString().ShouldBe(givenDate),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeConvertedToTheFirstDayOfTheMonth()
    {
        YearMonth.Create("2023-09")
            .Match(
                yearMonth => yearMonth.ToDateOnly().ShouldBe(new DateOnly(2023, 9, 1)),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenDate)
    {
        YearMonth.Create(givenDate)
            .Match(
                yearMonth => yearMonth.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Date cannot be empty")
            );
    }

    [Theory]
    [InlineData("2023-13")]
    [InlineData("2023-00")]
    [InlineData("2023-1")]
    [InlineData("23-01")]
    [InlineData("2023/01")]
    [InlineData("2023-01-15")]
    [InlineData("january 2023")]
    public void RejectDateWhenFormatIsInvalid(string givenDate)
    {
        YearMonth.Create(givenDate)
            .Match(
                yearMonth => yearMonth.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Date must have the format yyyy-MM")
            );
    }
}
