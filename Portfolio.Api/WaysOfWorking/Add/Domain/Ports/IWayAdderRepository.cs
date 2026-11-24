using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Add.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Add.Domain.Ports;

public interface IWayAdderRepository
{
    Task<Either<DomainError, Unit>> AddTranslation(WayToBeAdded toBeAdded);
}
