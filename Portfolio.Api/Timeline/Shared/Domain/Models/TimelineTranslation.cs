using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelineTranslation
{
    private readonly LanguageCode _languageCode;
    private readonly TimelineTitle _title;
    private readonly TimelineDescription _description;

    private TimelineTranslation(LanguageCode languageCode, TimelineTitle title, TimelineDescription description)
    {
        _languageCode = languageCode;
        _title = title;
        _description = description;
    }

    public static Either<Error, TimelineTranslation> Create(
        string givenLanguageCode,
        string givenTitle,
        string givenDescription
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from title in TimelineTitle.Create(givenTitle)
        from description in TimelineDescription.Create(givenDescription)
        select new TimelineTranslation(languageCode, title, description);

    public string GetLanguageCode() => _languageCode.ToString();

    public LanguageCode GetLanguageCodeModel() => _languageCode;

    public string GetTitle() => _title.ToString();

    public string GetDescription() => _description.ToString();
}
