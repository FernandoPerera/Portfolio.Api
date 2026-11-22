using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Find.Domain.Ports;

public interface IWayOfWorkingFinderRepository
{
    Task<Either<DomainError, List<WayOfWorkingFound>>> FindWays();
}
