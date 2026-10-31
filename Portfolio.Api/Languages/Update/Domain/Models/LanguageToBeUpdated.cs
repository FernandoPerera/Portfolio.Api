using Ci.Languages.Create.Domain.Models;
using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Update.Domain.Models;

public class LanguageToBeUpdated
{
    private readonly LanguageCode _languageCode;
    private readonly LanguageName _name;

    private LanguageToBeUpdated(LanguageCode languageCode, LanguageName name)
    {
        _languageCode = languageCode;
        _name = name;
    }

    public static Either<DomainError, LanguageToBeUpdated> Create(
        string givenLanguageCode,
        string givenName
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from name in LanguageName.Create(givenName)
        select new LanguageToBeUpdated(languageCode, name);

    public string GetLanguageCode() => _languageCode.ToString();

    public LanguageCode GetLanguageCodeModel() => _languageCode;

    public string GetName() => _name.ToString();
}
