using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using static LanguageExt.Prelude;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdated
{
    private readonly Guid _id;
    private readonly Organization _organization;
    private readonly TimelineCategory _category;
    private readonly TimelinePeriod _period;
    private readonly Technologies _technologies;
    private readonly LanguageCode _languageCode;
    private readonly TimelineTitle _title;
    private readonly TimelineDescription _description;

    private TimelineEntryToBeUpdated(
        Guid id,
        Organization organization,
        TimelineCategory category,
        TimelinePeriod period,
        Technologies technologies,
        LanguageCode languageCode,
        TimelineTitle title,
        TimelineDescription description
    )
    {
        _id = id;
        _organization = organization;
        _category = category;
        _period = period;
        _technologies = technologies;
        _languageCode = languageCode;
        _title = title;
        _description = description;
    }

    public static Either<DomainError, TimelineEntryToBeUpdated> Create(
        TimelineEntryFound current,
        string? givenOrganization,
        string? givenCategory,
        string? givenStartDate,
        string? givenEndDate,
        List<string>? givenTechnologies,
        string? givenTitle,
        string? givenDescription,
        string givenLanguageCode
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from translation in current.GetTranslation(languageCode)
        from organization in Optional(givenOrganization)
            .Match(Organization.Create, Organization.CreateUnsafe(current.GetOrganization()))
        from category in Optional(givenCategory)
            .Match(TimelineCategory.Create, TimelineCategory.CreateUnsafe(current.GetCategory()))
        from period in TimelinePeriod.Create(
            givenStartDate ?? current.GetStartDate(),
            givenEndDate ?? current.GetEndDate()
        )
        from technologies in Optional(givenTechnologies)
            .Match(Technologies.Create, Technologies.CreateUnsafe(current.GetTechnologies()))
        from title in Optional(givenTitle)
            .Match(TimelineTitle.Create, TimelineTitle.CreateUnsafe(translation.GetTitle()))
        from description in Optional(givenDescription)
            .Match(TimelineDescription.Create, TimelineDescription.CreateUnsafe(translation.GetDescription()))
        select new TimelineEntryToBeUpdated(
            current.GetId(),
            organization,
            category,
            period,
            technologies,
            languageCode,
            title,
            description
        );

    public Guid GetId() => _id;

    public string GetOrganization() => _organization.ToString();

    public string GetCategory() => _category.ToString();

    public DateOnly GetStartDate() => _period.GetStartDate().ToDateOnly();

    public Option<DateOnly> GetEndDate() => _period.GetEndDate().ToDateOnly();

    public List<string> GetTechnologies() => _technologies.GetValues();

    public string GetLanguageCode() => _languageCode.ToString();

    public string GetTitle() => _title.ToString();

    public string GetDescription() => _description.ToString();
}
