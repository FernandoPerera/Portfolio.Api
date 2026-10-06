using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using static LanguageExt.Prelude;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class ProfileFound
{
    private readonly Name _name;
    private readonly EnterpriseName _enterprise;
    private readonly List<ProfileTranslationFound> _translations;

    private ProfileFound(Name name, EnterpriseName enterprise, List<ProfileTranslationFound> translations)
    {
        _name = name;
        _enterprise = enterprise;
        _translations = translations;
    }

    public static Either<Error, ProfileFound> CreateUnsafe(
        string givenName,
        string? givenEnterpriseName,
        List<ProfileTranslationFound> translations
    ) =>
        from name in Name.CreateUnsafe(givenName)
        from enterpriseName in EnterpriseName.CreateUnsafe(givenEnterpriseName)
        select new ProfileFound(name, enterpriseName, translations);

    public string GetName() => _name.ToString();

    public string GetEnterprise() => _enterprise.ToString();

    public List<ProfileTranslationFound> GetTranslations() => _translations;

    public Either<Error, ProfileTranslationFound> GetTranslation(LanguageCode languageCode) =>
        Optional(_translations.FirstOrDefault(translation => translation.GetLanguageCode() == languageCode.ToString()))
            .ToEither(Error.New(new NotFoundException($"Profile translation for language '{languageCode}' was not found")));
}
