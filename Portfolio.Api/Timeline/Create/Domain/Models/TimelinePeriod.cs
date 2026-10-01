using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class TimelinePeriod
{
    private readonly StartDate _startDate;
    private readonly EndDate _endDate;

    private TimelinePeriod(StartDate startDate, EndDate endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public static Either<Error, TimelinePeriod> Create(string givenStartDate, string? givenEndDate) =>
        from startDate in StartDate.Create(givenStartDate)
        from endDate in EndDate.Create(givenEndDate)
        from _ in EnsureEndIsNotBeforeStart(startDate, endDate)
        select new TimelinePeriod(startDate, endDate);

    public DateOnly GetStartDate() => _startDate.ToDateOnly();

    public Option<DateOnly> GetEndDate() => _endDate.ToDateOnly();

    private static Either<Error, Unit> EnsureEndIsNotBeforeStart(StartDate startDate, EndDate endDate)
    {
        if (endDate.IsBefore(startDate))
        {
            return Error.New(new ValidationException("The end date cannot be earlier than the start date"));
        }

        return Unit.Default;
    }
}
