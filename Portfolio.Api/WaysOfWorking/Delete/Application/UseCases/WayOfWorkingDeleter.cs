using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Delete.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Delete.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.Delete.Application.UseCases;

public class WayOfWorkingDeleter(
    IWayOfWorkingByIdFinderRepository finder,
    IWayOfWorkingDeleterRepository deleter
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayOfWorkingDeleterInfo wayOfWorkingDeleterInfo)
    {
        return await (
            from id in WayOfWorkingId.Create(wayOfWorkingDeleterInfo.Id).ToAsync()
            from _ in finder.FindWay(id).ToAsync()
            from result in deleter.DeleteWay(id).ToAsync()
            select result
        );
    }
}
