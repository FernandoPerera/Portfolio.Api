using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Update.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Update.Domain.Models;
using Portfolio.Api.WaysOfWorking.Update.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.Update.Application.UseCases;

public class WayOfWorkingUpdater(
    IWayOfWorkingByIdFinderRepository finder,
    IWayOfWorkingUpdaterRepository updater
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayOfWorkingUpdaterInfo wayOfWorkingUpdaterInfo)
    {
        return await (
            from id in WayOfWorkingId.Create(wayOfWorkingUpdaterInfo.Id).ToAsync()
            from _ in finder.FindWay(id).ToAsync()
            from way in WayOfWorkingToBeUpdated.Create(
                wayOfWorkingUpdaterInfo.Id,
                wayOfWorkingUpdaterInfo.Illustration
            ).ToAsync()
            from result in updater.UpdateWay(way).ToAsync()
            select result
        );
    }
}
