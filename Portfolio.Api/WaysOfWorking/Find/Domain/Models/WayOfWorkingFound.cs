using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.WaysOfWorking.Find.Domain.Models;

public class WayOfWorkingFound
{
    private readonly WayOfWorkingId _id;
    private readonly Position _position;
    private readonly Illustration _illustration;
    private readonly WayTranslations _translations;

    private WayOfWorkingFound(
        WayOfWorkingId id,
        Position position,
        Illustration illustration,
        WayTranslations translations
    )
    {
        _id = id;
        _position = position;
        _illustration = illustration;
        _translations = translations;
    }

    public static Either<DomainError, WayOfWorkingFound> CreateUnsafe(
        Guid givenId,
        int givenPosition,
        string givenIllustration,
        List<WayTranslation> givenTranslations
    ) =>
        from id in WayOfWorkingId.CreateUnsafe(givenId)
        from position in Position.CreateUnsafe(givenPosition)
        from illustration in Illustration.CreateUnsafe(givenIllustration)
        from translations in WayTranslations.CreateUnsafe(givenTranslations)
        select new WayOfWorkingFound(id, position, illustration, translations);

    public Guid GetId() => _id.ToGuid();

    public int GetPosition() => _position.ToInt();

    public string GetIllustration() => _illustration.ToString();

    public List<WayTranslation> GetTranslations() => _translations.GetValues();

    public Either<DomainError, WayTranslation> GetTranslation(LanguageCode languageCode) =>
        Optional(_translations.GetValues().FirstOrDefault(translation => translation.GetLanguageCode() == languageCode.ToString()))
            .ToEither((DomainError)new NotFoundError($"Way of working translation for language '{languageCode}' was not found"));

    public Either<DomainError, Unit> EnsureTranslationIsNew(LanguageCode languageCode)
    {
        var hasTranslation = _translations.GetValues()
            .Any(translation => translation.GetLanguageCode() == languageCode.ToString());
        if (hasTranslation)
        {
            return new ConflictError($"Way of working already has a translation for language '{languageCode}'");
        }

        return Unit.Default;
    }
}
