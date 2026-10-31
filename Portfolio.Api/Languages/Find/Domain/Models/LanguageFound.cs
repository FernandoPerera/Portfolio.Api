using Ci.Languages.Create.Domain.Models;
using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Find.Domain.Models;

public class LanguageFound
{
    private readonly LanguageCode _languageCode;
    private readonly LanguageName _name;

    private LanguageFound(LanguageCode languageCode, LanguageName name)
    {
        _languageCode = languageCode;
        _name = name;
    }

    public static Either<DomainError, LanguageFound> CreateUnsafe(
        string givenLanguageCode,
        string givenName
    ) =>
        from languageCode in LanguageCode.CreateUnsafe(givenLanguageCode)
        from name in LanguageName.CreateUnsafe(givenName)
        select new LanguageFound(languageCode, name);

    public string GetLanguageCode() => _languageCode.ToString();

    public string GetName() => _name.ToString();
}
