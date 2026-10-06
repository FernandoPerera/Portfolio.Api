using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdated
{
    private readonly Guid _id;
    private readonly Organization _organization;
    private readonly TimelineCategory _category;
    private readonly TimelinePeriod _period;
    private readonly Technologies _technologies;
    private readonly TimelineTranslations _translations;

    private TimelineEntryToBeUpdated(
        Guid id,
        Organization organization,
        TimelineCategory category,
        TimelinePeriod period,
        Technologies technologies,
        TimelineTranslations translations
    )
    {
        _id = id;
        _organization = organization;
        _category = category;
        _period = period;
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
        from organization in Optional(givenOrganization)
            .Match(Organization.Create, () => Organization.CreateUnsafe(current.GetOrganization()))
        from category in Optional(givenCategory)
            .Match(TimelineCategory.Create, () => TimelineCategory.CreateUnsafe(current.GetCategory()))
        from period in TimelinePeriod.Create(
            givenStartDate ?? current.GetStartDate(),
            givenEndDate ?? current.GetEndDate()
        )
        from technologies in Optional(givenTechnologies)
            .Match(Technologies.Create, () => Technologies.CreateUnsafe(current.GetTechnologies()))
        from translations in givenTranslations
            .Match(TimelineTranslations.Create, () => TimelineTranslations.CreateUnsafe(current.GetTranslations()))
        select new TimelineEntryToBeUpdated(current.GetId(), organization, category, period, technologies, translations);

    public Guid GetId() => _id;

    public string GetOrganization() => _organization.ToString();

    public string GetCategory() => _category.ToString();

    public DateOnly GetStartDate() => _period.GetStartDate().ToDateOnly();

    public Option<DateOnly> GetEndDate() => _period.GetEndDate().ToDateOnly();

    public List<string> GetTechnologies() => _technologies.GetValues();

    public List<TimelineTranslation> GetTranslations() => _translations.GetValues();
}
