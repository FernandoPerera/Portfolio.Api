using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Add.Domain.Models;

public class WayToBeAdded
{
    private readonly WayOfWorkingId _wayId;
    private readonly WayTranslation _translation;

    private WayToBeAdded(WayOfWorkingId wayId, WayTranslation translation)
    {
        _wayId = wayId;
        _translation = translation;
    }

    public static Either<DomainError, WayToBeAdded> Create(
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
        select new WayToBeAdded(wayId, translation);

    public Guid GetWayId() => _wayId.ToGuid();

    public WayTranslation GetTranslation() => _translation;
}
