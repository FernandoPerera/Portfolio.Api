using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class ProfileTranslationFound
{
    private readonly LanguageCode _languageCode;
    private readonly Presentation _presentation;
    private readonly JobRole _jobRole;

    private ProfileTranslationFound(LanguageCode languageCode, Presentation presentation, JobRole jobRole)
    {
        _languageCode = languageCode;
        _presentation = presentation;
        _jobRole = jobRole;
    }

    public static Either<DomainError, ProfileTranslationFound> CreateUnsafe(
        string givenLanguageCode,
        string givenPresentation,
        string givenJobRole
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from presentation in Presentation.CreateUnsafe(givenPresentation)
        from jobRole in JobRole.CreateUnsafe(givenJobRole)
        select new ProfileTranslationFound(languageCode, presentation, jobRole);

    public string GetLanguageCode() => _languageCode.ToString();

    public string GetPresentation() => _presentation.ToString();

    public string GetRole() => _jobRole.ToString();
}
