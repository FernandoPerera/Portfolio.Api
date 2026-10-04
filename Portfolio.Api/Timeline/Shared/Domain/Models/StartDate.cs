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

    public static Either<Error, StartDate> CreateUnsafe(DateOnly startDate) =>
        YearMonth.CreateUnsafe(startDate).Map(yearMonth => new StartDate(yearMonth));

    public override string ToString() => _startDate.ToString();

    public DateOnly ToDateOnly() => _startDate.ToDateOnly();
}
