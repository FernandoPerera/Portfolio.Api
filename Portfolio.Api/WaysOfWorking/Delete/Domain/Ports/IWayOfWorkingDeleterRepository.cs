using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Delete.Domain.Ports;

public interface IWayOfWorkingDeleterRepository
{
    Task<Either<DomainError, Unit>> DeleteWay(WayOfWorkingId id);
}
