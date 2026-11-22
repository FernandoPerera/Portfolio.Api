using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

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

    public bool HasTranslationIn(LanguageCode languageCode) =>
        _translations.GetValues().Any(translation => translation.GetLanguageCode() == languageCode.ToString());
}
