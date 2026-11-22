using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Ports;

public interface IWayTranslationAdderRepository
{
    Task<Either<DomainError, Unit>> AddTranslation(WayTranslationToBeAdded translationToBeAdded);
}
