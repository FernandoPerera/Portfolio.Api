using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.UseCases;

public class WayTranslationUpdater(
    IWayOfWorkingByIdFinderRepository finder,
    IWayTranslationUpdaterRepository updater
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayTranslationUpdaterInfo wayTranslationUpdaterInfo)
    {
        return await (
            from id in WayOfWorkingId.Create(wayTranslationUpdaterInfo.Id).ToAsync()
            from current in finder.FindWay(id).ToAsync()
            from translation in WayTranslationToBeUpdated.Create(
                current,
                wayTranslationUpdaterInfo.LanguageCode,
                wayTranslationUpdaterInfo.Label,
                wayTranslationUpdaterInfo.Caption,
                wayTranslationUpdaterInfo.Title,
                wayTranslationUpdaterInfo.Description,
                wayTranslationUpdaterInfo.Points
            ).ToAsync()
            from result in updater.UpdateTranslation(translation).ToAsync()
            select result
        );
    }
}
