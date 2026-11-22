using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayTranslations
{
    private readonly List<WayTranslation> _translations;

    private WayTranslations(List<WayTranslation> translations)
    {
        _translations = translations;
    }

    public static Either<DomainError, WayTranslations> Create(List<WayTranslationInput> givenTranslations) =>
        from translations in givenTranslations
            .Select(translation => WayTranslation.Create(
                translation.LanguageCode,
                translation.Label,
                translation.Caption,
                translation.Title,
                translation.Description,
                translation.Points
            ))
            .Sequence()
        from validTranslations in EnsureAreValid(translations.ToList())
        select new WayTranslations(validTranslations);

    public static Either<DomainError, WayTranslations> CreateUnsafe(List<WayTranslation> translations) =>
        new WayTranslations(translations);

    public List<WayTranslation> GetValues() => _translations;

    private static Either<DomainError, List<WayTranslation>> EnsureAreValid(List<WayTranslation> translations)
    {
        if (translations.Count == 0)
        {
            return new ValidationError("At least one translation is required");
        }

        var hasRepeatedLanguage = translations
            .GroupBy(translation => translation.GetLanguageCode())
            .Any(group => group.Count() > 1);
        if (hasRepeatedLanguage)
        {
            return new ValidationError("Translations cannot repeat the same language");
        }

        return translations;
    }
}
