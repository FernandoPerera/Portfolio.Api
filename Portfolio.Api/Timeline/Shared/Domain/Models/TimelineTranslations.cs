using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelineTranslations
{
    private readonly List<TimelineTranslation> _translations;

    private TimelineTranslations(List<TimelineTranslation> translations)
    {
        _translations = translations;
    }

    public static Either<Error, TimelineTranslations> Create(List<TimelineTranslation> translations)
    {
        if (translations.Count == 0)
        {
            return Error.New(new ValidationException("At least one translation is required"));
        }

        var hasRepeatedLanguage = translations
            .GroupBy(translation => translation.GetLanguageCode())
            .Any(group => group.Count() > 1);
        if (hasRepeatedLanguage)
        {
            return Error.New(new ValidationException("Translations cannot repeat the same language"));
        }

        return new TimelineTranslations(translations);
    }

    public static Either<Error, TimelineTranslations> CreateUnsafe(List<TimelineTranslation> translations) =>
        new TimelineTranslations(translations);

    public List<TimelineTranslation> GetValues() => _translations;
}
