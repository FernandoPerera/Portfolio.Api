using LanguageExt;
using Ci.Languages.Create.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Create.Domain.Models;

public class LanguageToBeCreated
{
    private readonly LanguageCode _languageCode;
    private readonly LanguageName _name;

    private LanguageToBeCreated(LanguageCode languageCode, LanguageName name)
    {
        _languageCode = languageCode;
        _name = name;
    }

    public static Either<DomainError, LanguageToBeCreated> Create(
        string givenLanguageCode,
        string givenName
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from name in LanguageName.Create(givenName)
        select new LanguageToBeCreated(languageCode, name);

    public string GetLanguageCode() => _languageCode.ToString();

    public string GetName() => _name.ToString();
}
