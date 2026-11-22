using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Update.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Update.Domain.Ports;

public interface IWayOfWorkingUpdaterRepository
{
    Task<Either<DomainError, Unit>> UpdateWay(WayOfWorkingToBeUpdated wayOfWorkingToBeUpdated);
}
