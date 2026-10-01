using LanguageExt;
using LanguageExt.Common;

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
}
