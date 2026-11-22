using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.WaysOfWorking.Create.Domain.Models;

public class WayOfWorkingToBeCreated
{
    private readonly Illustration _illustration;
    private readonly WayTranslations _translations;

    private WayOfWorkingToBeCreated(Illustration illustration, WayTranslations translations)
    {
        _illustration = illustration;
        _translations = translations;
    }

    public static Either<DomainError, WayOfWorkingToBeCreated> Create(
        string givenIllustration,
        List<WayTranslationInput> givenTranslations
    ) =>
        from illustration in Illustration.Create(givenIllustration)
        from translations in WayTranslations.Create(givenTranslations)
        select new WayOfWorkingToBeCreated(illustration, translations);

    public string GetIllustration() => _illustration.ToString();

    public List<WayTranslation> GetTranslations() => _translations.GetValues();
}
