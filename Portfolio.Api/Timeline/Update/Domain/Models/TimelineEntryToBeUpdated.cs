using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdated
{
    private readonly Guid _id;
    private readonly Option<Organization> _organization;
    private readonly Option<TimelineCategory> _category;
    private readonly Option<StartDate> _startDate;
    private readonly Option<EndDate> _endDate;
    private readonly Option<Technologies> _technologies;
    private readonly Option<TimelineTranslations> _translations;

    private TimelineEntryToBeUpdated(
        Guid id,
        Option<Organization> organization,
        Option<TimelineCategory> category,
        Option<StartDate> startDate,
        Option<EndDate> endDate,
        Option<Technologies> technologies,
        Option<TimelineTranslations> translations
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
        Option<List<TimelineTranslation>> givenTranslations
    ) =>
        from organization in Optional(givenOrganization).Map(Organization.Create).Sequence()
        from category in Optional(givenCategory).Map(TimelineCategory.Create).Sequence()
        from startDate in Optional(givenStartDate).Map(StartDate.Create).Sequence()
        from endDate in Optional(givenEndDate).Map(EndDate.Create).Sequence()
        from _ in EnsurePeriodIsValid(current, givenStartDate, givenEndDate)
        from technologies in Optional(givenTechnologies).Map(Technologies.Create).Sequence()
        from translations in givenTranslations.Map(TimelineTranslations.Create).Sequence()
        select new TimelineEntryToBeUpdated(
            current.GetId(),
            organization,
            category,
            startDate,
            endDate,
            technologies,
            translations
        );

    public Guid GetId() => _id;

    public Option<string> GetOrganization() => _organization.Map(organization => organization.ToString());

    public Option<string> GetCategory() => _category.Map(category => category.ToString());

    public Option<DateOnly> GetStartDate() => _startDate.Map(startDate => startDate.ToDateOnly());

    public Option<DateOnly> GetEndDate() => _endDate.Bind(endDate => endDate.ToDateOnly());

    public Option<List<string>> GetTechnologies() => _technologies.Map(technologies => technologies.GetValues());

    public Option<List<TimelineTranslation>> GetTranslations() => _translations.Map(translations => translations.GetValues());

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
