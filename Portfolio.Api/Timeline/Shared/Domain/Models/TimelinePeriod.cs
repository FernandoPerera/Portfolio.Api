using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelinePeriod
{
    private readonly StartDate _startDate;
    private readonly EndDate _endDate;

    private TimelinePeriod(StartDate startDate, EndDate endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public static Either<DomainError, TimelinePeriod> Create(string givenStartDate, string? givenEndDate) =>
        from startDate in StartDate.Create(givenStartDate)
        from endDate in EndDate.Create(givenEndDate)
        from _ in EnsureEndIsNotBeforeStart(startDate, endDate)
        select new TimelinePeriod(startDate, endDate);

    public static Either<DomainError, TimelinePeriod> CreateUnsafe(DateOnly givenStartDate, DateOnly? givenEndDate) =>
        from startDate in StartDate.CreateUnsafe(givenStartDate)
        from endDate in EndDate.CreateUnsafe(givenEndDate)
        select new TimelinePeriod(startDate, endDate);

    public StartDate GetStartDate() => _startDate;

    public EndDate GetEndDate() => _endDate;

    private static Either<DomainError, Unit> EnsureEndIsNotBeforeStart(StartDate startDate, EndDate endDate)
    {
        if (endDate.IsBefore(startDate))
        {
            return new ValidationError("The end date cannot be earlier than the start date");
        }

        return Unit.Default;
    }
}
