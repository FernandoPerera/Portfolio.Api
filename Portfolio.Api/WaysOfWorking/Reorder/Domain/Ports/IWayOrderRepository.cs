using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Reorder.Domain.Ports;

public interface IWayOrderRepository
{
    Task<Either<DomainError, Unit>> ReorderWays(WayOrder order);
}
