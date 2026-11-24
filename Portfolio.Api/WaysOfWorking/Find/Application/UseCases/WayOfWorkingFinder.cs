using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.Find.Application.UseCases;

public class WayOfWorkingFinder(IWayOfWorkingFinderRepository repository)
{
    public async Task<Either<DomainError, List<WayOfWorkingFound>>> Execute()
    {
        return await (
            from ways in repository.FindWays().ToAsync()
            select ways
        );
    }
}
