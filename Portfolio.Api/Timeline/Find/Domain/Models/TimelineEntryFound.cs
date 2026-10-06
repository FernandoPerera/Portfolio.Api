using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.Timeline.Find.Domain.Models;

public class TimelineEntryFound
{
    private readonly Guid _id;
    private readonly Organization _organization;
    private readonly TimelineCategory _category;
    private readonly TimelinePeriod _period;
    private readonly Technologies _technologies;
    private readonly TimelineTranslations _translations;

    private TimelineEntryFound(
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

    public static Either<Error, TimelineEntryFound> CreateUnsafe(
        Guid givenId,
        string givenOrganization,
        string givenCategory,
        DateOnly givenStartDate,
        DateOnly? givenEndDate,
        List<string> givenTechnologies,
        List<TimelineTranslation> givenTranslations
    ) =>
        from organization in Organization.CreateUnsafe(givenOrganization)
        from category in TimelineCategory.CreateUnsafe(givenCategory)
        from period in TimelinePeriod.CreateUnsafe(givenStartDate, givenEndDate)
        from technologies in Technologies.CreateUnsafe(givenTechnologies)
        from translations in TimelineTranslations.CreateUnsafe(givenTranslations)
        select new TimelineEntryFound(givenId, organization, category, period, technologies, translations);

    public Guid GetId() => _id;

    public string GetOrganization() => _organization.ToString();

    public string GetCategory() => _category.ToString();

    public string GetStartDate() => _period.GetStartDate().ToString();

    public string? GetEndDate() => _period.GetEndDate().ToString();

    public List<string> GetTechnologies() => _technologies.GetValues();

    public List<TimelineTranslation> GetTranslations() => _translations.GetValues();

    public Either<Error, TimelineTranslation> GetTranslation(LanguageCode languageCode) =>
        Optional(_translations.GetValues().FirstOrDefault(translation => translation.GetLanguageCode() == languageCode.ToString()))
            .ToEither(Error.New(new NotFoundException($"Timeline translation for language '{languageCode}' was not found")));
}
