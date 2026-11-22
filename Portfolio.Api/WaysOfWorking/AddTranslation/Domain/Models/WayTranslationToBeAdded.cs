using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;

public class WayTranslationToBeAdded
{
    private readonly WayOfWorkingId _wayId;
    private readonly WayTranslation _translation;

    private WayTranslationToBeAdded(WayOfWorkingId wayId, WayTranslation translation)
    {
        _wayId = wayId;
        _translation = translation;
    }

    public static Either<DomainError, WayTranslationToBeAdded> Create(
        WayOfWorkingFound current,
        WayTranslationInput givenTranslation
    ) =>
        from languageCode in LanguageCode.Create(givenTranslation.LanguageCode)
        from _ in EnsureTranslationIsNew(current, languageCode)
        from wayId in WayOfWorkingId.CreateUnsafe(current.GetId())
        from translation in WayTranslation.Create(
            languageCode.ToString(),
            givenTranslation.Label,
            givenTranslation.Caption,
            givenTranslation.Title,
            givenTranslation.Description,
            givenTranslation.Points
        )
        select new WayTranslationToBeAdded(wayId, translation);

    public Guid GetWayId() => _wayId.ToGuid();

    public WayTranslation GetTranslation() => _translation;

    private static Either<DomainError, Unit> EnsureTranslationIsNew(WayOfWorkingFound current, LanguageCode languageCode)
    {
        if (current.HasTranslationIn(languageCode))
        {
            return new ConflictError($"Way of working already has a translation for language '{languageCode}'");
        }

        return Unit.Default;
    }
}
