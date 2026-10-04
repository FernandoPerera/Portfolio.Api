using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class StartDate
{
    private readonly YearMonth _startDate;

    private StartDate(YearMonth startDate)
    {
        _startDate = startDate;
    }

    public static Either<Error, StartDate> Create(string startDate) =>
        YearMonth.Create(startDate).Map(yearMonth => new StartDate(yearMonth));

    public DateOnly ToDateOnly() => _startDate.ToDateOnly();
}
