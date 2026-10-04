using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class EndDate
{
    private readonly Option<YearMonth> _endDate;

    private EndDate(Option<YearMonth> endDate)
    {
        _endDate = endDate;
    }

    public static Either<Error, EndDate> Create(string? endDate)
    {
        if (endDate is null)
        {
            return new EndDate(Option<YearMonth>.None);
        }

        return YearMonth.Create(endDate).Map(yearMonth => new EndDate(yearMonth));
    }

    public bool IsBefore(StartDate startDate) =>
        _endDate.Match(endDate => endDate.ToDateOnly() < startDate.ToDateOnly(), () => false);

    public Option<DateOnly> ToDateOnly() => _endDate.Map(endDate => endDate.ToDateOnly());
}
