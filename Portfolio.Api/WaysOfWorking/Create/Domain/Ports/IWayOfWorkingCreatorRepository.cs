using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Create.Domain.Ports;

public interface IWayOfWorkingCreatorRepository
{
    Task<Either<DomainError, Unit>> CreateWay(WayOfWorkingToBeCreated wayOfWorkingToBeCreated);
}
