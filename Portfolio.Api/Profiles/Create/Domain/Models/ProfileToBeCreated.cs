using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class ProfileToBeCreated
{
    private readonly Name _name;
    private readonly Presentation _presentation;
    private readonly JobRole _jobRole;
    private readonly EnterpriseName _enterprise;

    private ProfileToBeCreated(Name name, Presentation presentation, JobRole jobRole, EnterpriseName enterprise)
    {
        _name = name;
        _presentation = presentation;
        _jobRole = jobRole;
        _enterprise = enterprise;
    }

    public static Either<Error, ProfileToBeCreated> Create(
        string givenName,
        string givenPresentation,
        string givenJobRole,
        string givenEnterpriseName
    ) =>
        from name in Name.Create(givenName)
        from presentation in Presentation.Create(givenPresentation)
        from jobRole in JobRole.Create(givenJobRole)
        from enterpriseName in EnterpriseName.Create(givenEnterpriseName)
        select new ProfileToBeCreated(name, presentation, jobRole, enterpriseName);

    public string GetName() => _name.ToString();

    public string GetPresentation() => _presentation.ToString();

    public string GetRole() => _jobRole.ToString();

    public string GetEnterprise() => _enterprise.ToString();
}