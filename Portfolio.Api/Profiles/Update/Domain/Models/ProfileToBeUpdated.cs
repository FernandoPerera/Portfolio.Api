using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using static LanguageExt.Prelude;
using EnterpriseName = Portfolio.Api.Profiles.Shared.Domain.Models.EnterpriseName;
using JobRole = Portfolio.Api.Profiles.Shared.Domain.Models.JobRole;
using Name = Portfolio.Api.Profiles.Shared.Domain.Models.Name;
using Presentation = Portfolio.Api.Profiles.Shared.Domain.Models.Presentation;

namespace Portfolio.Api.Profiles.Update.Domain.Models;

public class ProfileToBeUpdated
{
    private readonly Name _name;
    private readonly EnterpriseName _enterprise;
    private readonly JobRole _jobRole;
    private readonly Presentation _presentation;
    private readonly LanguageCode _languageCode;

    private ProfileToBeUpdated(
        Name name,
        EnterpriseName enterprise,
        JobRole jobRole,
        Presentation presentation,
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
        ProfileFound current,
        string? givenName,
        string? givenEnterpriseName,
        string? givenJobRole,
        string? givenPresentation,
        string givenLanguageCode
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from translation in current.GetTranslation(languageCode)
        from name in Optional(givenName)
            .Match(Name.Create, Name.CreateUnsafe(current.GetName()))
        from enterpriseName in Optional(givenEnterpriseName)
            .Match(EnterpriseName.Create, EnterpriseName.CreateUnsafe(current.GetEnterprise()))
        from jobRole in Optional(givenJobRole)
            .Match(JobRole.Create, JobRole.CreateUnsafe(translation.GetRole()))
        from presentation in Optional(givenPresentation)
            .Match(Presentation.Create, Presentation.CreateUnsafe(translation.GetPresentation()))
        select new ProfileToBeUpdated(name, enterpriseName, jobRole, presentation, languageCode);

    public string GetName() => _name.ToString();

    public string GetEnterprise() => _enterprise.ToString();

    public string GetRole() => _jobRole.ToString();

    public string GetPresentation() => _presentation.ToString();

    public string GetLanguageCode() => _languageCode.ToString();
}
