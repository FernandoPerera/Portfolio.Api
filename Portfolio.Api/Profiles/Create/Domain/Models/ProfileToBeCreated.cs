using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class ProfileToBeCreated
{
    private readonly Name _name;
    private readonly Presentation _presentation;
    private readonly JobRole _jobRole;
    private readonly EnterpriseName _enterprise;
    private readonly LanguageCode _languageCode;

    private ProfileToBeCreated(
        Name name,
        Presentation presentation,
        JobRole jobRole,
        EnterpriseName enterprise,
        LanguageCode languageCode
    )
    {
        _name = name;
        _presentation = presentation;
        _jobRole = jobRole;
        _enterprise = enterprise;
        _languageCode = languageCode;
    }

    public static Either<Error, ProfileToBeCreated> Create(
        string givenName,
        string givenPresentation,
        string givenJobRole,
        string? givenEnterpriseName,
        string givenLanguageCode
    ) =>
        from name in Name.Create(givenName)
        from presentation in Presentation.Create(givenPresentation)
        from jobRole in JobRole.Create(givenJobRole)
        from enterpriseName in EnterpriseName.Create(givenEnterpriseName)
        from languageCode in LanguageCode.Create(givenLanguageCode)
        select new ProfileToBeCreated(name, presentation, jobRole, enterpriseName, languageCode);

    public string GetName() => _name.ToString();

    public string GetPresentation() => _presentation.ToString();

    public string GetRole() => _jobRole.ToString();

    public string GetEnterprise() => _enterprise.ToString();

    public string GetLanguageCode() => _languageCode.ToString();

    public LanguageCode GetLanguageCodeModel() => _languageCode;
}