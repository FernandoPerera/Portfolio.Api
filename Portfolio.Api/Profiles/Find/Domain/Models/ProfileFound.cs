using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class ProfileFound
{
    private readonly Name _name;
    private readonly Presentation _presentation;
    private readonly JobRole _jobRole;
    private readonly EnterpriseName _enterprise;

    private ProfileFound(Name name, Presentation presentation, JobRole jobRole, EnterpriseName enterprise)
    {
        _name = name;
        _presentation = presentation;
        _jobRole = jobRole;
        _enterprise = enterprise;
    }

    public static Either<Error, ProfileFound> CreateUnsafe(
        string givenName,
        string givenPresentation,
        string givenJobRole,
        string? givenEnterpriseName
    ) =>
        from name in Name.CreateUnsafe(givenName)
        from presentation in Presentation.CreateUnsafe(givenPresentation)
        from jobRole in JobRole.CreateUnsafe(givenJobRole)
        from enterpriseName in EnterpriseName.CreateUnsafe(givenEnterpriseName)
        select new ProfileFound(name, presentation, jobRole, enterpriseName);

    public string GetName() => _name.ToString();

    public string GetPresentation() => _presentation.ToString();

    public string GetRole() => _jobRole.ToString();

    public string GetEnterprise() => _enterprise.ToString();
}
