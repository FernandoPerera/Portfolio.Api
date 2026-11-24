using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Ports;

public interface IWayTranslationUpdaterRepository
{
    Task<Either<DomainError, Unit>> UpdateTranslation(WayTranslationToBeUpdated translationToBeUpdated);
}
