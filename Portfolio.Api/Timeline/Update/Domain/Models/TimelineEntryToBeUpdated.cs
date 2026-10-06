using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdated
{
    private readonly Guid _id;
    private readonly Organization? _organization;
    private readonly TimelineCategory? _category;
    private readonly StartDate? _startDate;
    private readonly EndDate? _endDate;
    private readonly Technologies? _technologies;
    private readonly TimelineTranslations? _translations;

    private TimelineEntryToBeUpdated(
        Guid id,
        Organization? organization,
        TimelineCategory? category,
        StartDate? startDate,
        EndDate? endDate,
        Technologies? technologies,
        TimelineTranslations? translations
    )
    {
        _id = id;
        _organization = organization;
        _category = category;
        _startDate = startDate;
        _endDate = endDate;
        _technologies = technologies;
        _translations = translations;
    }

    public static Either<Error, TimelineEntryToBeUpdated> Create(
        TimelineEntryFound current,
        string? givenOrganization,
        string? givenCategory,
        string? givenStartDate,
        string? givenEndDate,
        List<string>? givenTechnologies,
        List<TimelineTranslation>? givenTranslations
    ) =>
        from organization in CreateIfGiven(givenOrganization, Organization.Create)
        from category in CreateIfGiven(givenCategory, TimelineCategory.Create)
        from startDate in CreateIfGiven(givenStartDate, StartDate.Create)
        from endDate in CreateIfGiven(givenEndDate, EndDate.Create)
        from _ in EnsurePeriodIsValid(current, givenStartDate, givenEndDate)
        from technologies in CreateIfGiven(givenTechnologies, Technologies.Create)
        from translations in CreateIfGiven(givenTranslations, TimelineTranslations.Create)
        select new TimelineEntryToBeUpdated(
            current.GetId(),
            organization.IfNoneUnsafe((Organization?)null),
            category.IfNoneUnsafe((TimelineCategory?)null),
            startDate.IfNoneUnsafe((StartDate?)null),
            endDate.IfNoneUnsafe((EndDate?)null),
            technologies.IfNoneUnsafe((Technologies?)null),
            translations.IfNoneUnsafe((TimelineTranslations?)null)
        );

    public Guid GetId() => _id;

    public string? GetOrganization() => _organization?.ToString();

    public string? GetCategory() => _category?.ToString();

    public DateOnly? GetStartDate() => _startDate?.ToDateOnly();

    public DateOnly? GetEndDate() => _endDate?.ToDateOnly().ToNullable();

    public List<string>? GetTechnologies() => _technologies?.GetValues();

    public List<TimelineTranslation>? GetTranslations() => _translations?.GetValues();

    private static Either<Error, Option<TValue>> CreateIfGiven<TGiven, TValue>(
        TGiven? given,
        Func<TGiven, Either<Error, TValue>> create
    )
        where TGiven : class
        where TValue : class
    {
        if (given is null)
        {
            return Option<TValue>.None;
        }

        return create(given).Map(Option<TValue>.Some);
    }

    private static Either<Error, Unit> EnsurePeriodIsValid(
        TimelineEntryFound current,
        string? givenStartDate,
        string? givenEndDate
    )
    {
        if (givenStartDate is null && givenEndDate is null)
        {
            return Unit.Default;
        }

        return TimelinePeriod
            .Create(givenStartDate ?? current.GetStartDate(), givenEndDate ?? current.GetEndDate())
            .Map(_ => Unit.Default);
    }
}
