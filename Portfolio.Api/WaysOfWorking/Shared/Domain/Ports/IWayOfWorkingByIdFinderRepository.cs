using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;

public interface IWayOfWorkingByIdFinderRepository
{
    Task<Either<DomainError, WayOfWorkingFound>> FindWay(WayOfWorkingId id);
}
