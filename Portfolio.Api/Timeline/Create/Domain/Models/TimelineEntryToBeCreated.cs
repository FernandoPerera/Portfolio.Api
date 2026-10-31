using LanguageExt;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class TimelineEntryToBeCreated
{
    private readonly Organization _organization;
    private readonly TimelineCategory _category;
    private readonly TimelinePeriod _period;
    private readonly Technologies _technologies;
    private readonly TimelineTranslations _translations;

    private TimelineEntryToBeCreated(
        Organization organization,
        TimelineCategory category,
        TimelinePeriod period,
        Technologies technologies,
        TimelineTranslations translations
    )
    {
        _organization = organization;
        _category = category;
        _period = period;
        _technologies = technologies;
        _translations = translations;
    }

    public static Either<DomainError, TimelineEntryToBeCreated> Create(
        string givenOrganization,
        string givenCategory,
        string givenStartDate,
        string? givenEndDate,
        List<string> givenTechnologies,
        List<TimelineTranslation> givenTranslations
    ) =>
        from organization in Organization.Create(givenOrganization)
        from category in TimelineCategory.Create(givenCategory)
        from period in TimelinePeriod.Create(givenStartDate, givenEndDate)
        from technologies in Technologies.Create(givenTechnologies)
        from translations in TimelineTranslations.Create(givenTranslations)
        select new TimelineEntryToBeCreated(organization, category, period, technologies, translations);

    public string GetOrganization() => _organization.ToString();

    public string GetCategory() => _category.ToString();

    public DateOnly GetStartDate() => _period.GetStartDate().ToDateOnly();

    public Option<DateOnly> GetEndDate() => _period.GetEndDate().ToDateOnly();

    public List<string> GetTechnologies() => _technologies.GetValues();

    public List<TimelineTranslation> GetTranslations() => _translations.GetValues();
}
