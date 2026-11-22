using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
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
        Guid givenWayId,
        WayTranslationInput givenTranslation
    ) =>
        from wayId in WayOfWorkingId.Create(givenWayId)
        from translation in WayTranslation.Create(
            givenTranslation.LanguageCode,
            givenTranslation.Label,
            givenTranslation.Caption,
            givenTranslation.Title,
            givenTranslation.Description,
            givenTranslation.Points
        )
        select new WayTranslationToBeAdded(wayId, translation);

    public Guid GetWayId() => _wayId.ToGuid();

    public WayTranslation GetTranslation() => _translation;
}
