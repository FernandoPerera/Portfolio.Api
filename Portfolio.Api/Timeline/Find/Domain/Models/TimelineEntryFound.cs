using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Find.Domain.Models;

public class TimelineEntryFound
{
    private readonly Organization _organization;
    private readonly TimelineCategory _category;
    private readonly DateOnly _startDate;
    private readonly Option<DateOnly> _endDate;
    private readonly List<string> _technologies;
    private readonly List<TimelineTranslation> _translations;

    private TimelineEntryFound(
        Organization organization,
        TimelineCategory category,
        DateOnly startDate,
        Option<DateOnly> endDate,
        List<string> technologies,
        List<TimelineTranslation> translations
    )
    {
        _organization = organization;
        _category = category;
        _startDate = startDate;
        _endDate = endDate;
        _technologies = technologies;
        _translations = translations;
    }

    public static Either<Error, TimelineEntryFound> Create(
        string givenOrganization,
        string givenCategory,
        DateOnly startDate,
        DateOnly? endDate,
        List<string> technologies,
        List<TimelineTranslation> translations
    ) =>
        from organization in Organization.Create(givenOrganization)
        from category in TimelineCategory.Create(givenCategory)
        select new TimelineEntryFound(
            organization,
            category,
            startDate,
            Prelude.Optional(endDate),
            technologies,
            translations
        );

    public string GetOrganization() => _organization.ToString();

    public string GetCategory() => _category.ToString();

    public DateOnly GetStartDate() => _startDate;

    public Option<DateOnly> GetEndDate() => _endDate;

    public List<string> GetTechnologies() => _technologies;

    public List<TimelineTranslation> GetTranslations() => _translations;
}
