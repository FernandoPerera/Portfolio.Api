using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.Profiles.Update.Domain.Models;

public class ProfileToBeUpdated
{
    private readonly Option<Name> _name;
    private readonly Option<EnterpriseName> _enterprise;
    private readonly Option<JobRole> _jobRole;
    private readonly Option<Presentation> _presentation;
    private readonly LanguageCode _languageCode;

    private ProfileToBeUpdated(
        Option<Name> name,
        Option<EnterpriseName> enterprise,
        Option<JobRole> jobRole,
        Option<Presentation> presentation,
        LanguageCode languageCode
    )
    {
        _name = name;
        _enterprise = enterprise;
        _jobRole = jobRole;
        _presentation = presentation;
        _languageCode = languageCode;
    }

    public static Either<Error, ProfileToBeUpdated> Create(
        string? givenName,
        string? givenEnterpriseName,
        string? givenJobRole,
        string? givenPresentation,
        string givenLanguageCode
    ) =>
        from name in Optional(givenName).Map(Name.Create).Sequence()
        from enterpriseName in Optional(givenEnterpriseName).Map(EnterpriseName.Create).Sequence()
        from jobRole in Optional(givenJobRole).Map(JobRole.Create).Sequence()
        from presentation in Optional(givenPresentation).Map(Presentation.Create).Sequence()
        from languageCode in LanguageCode.Create(givenLanguageCode)
        select new ProfileToBeUpdated(name, enterpriseName, jobRole, presentation, languageCode);

    public Option<string> GetName() => _name.Map(name => name.ToString());

    public Option<string> GetEnterprise() => _enterprise.Map(enterprise => enterprise.ToString());

    public Option<string> GetRole() => _jobRole.Map(jobRole => jobRole.ToString());

    public Option<string> GetPresentation() => _presentation.Map(presentation => presentation.ToString());

    public string GetLanguageCode() => _languageCode.ToString();
}
