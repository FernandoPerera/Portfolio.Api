using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Add.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Add.Domain.Models;
using Portfolio.Api.WaysOfWorking.Add.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.Add.Application.UseCases;

public class WayAdder(
    IWayOfWorkingByIdFinderRepository finder,
    ILanguageExistenceRepository languageExistence,
    IWayAdderRepository adder
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayAdderInfo wayAdderInfo)
    {
        return await (
            from id in WayOfWorkingId.Create(wayAdderInfo.Id).ToAsync()
            from current in finder.FindWay(id).ToAsync()
            from languageCode in LanguageCode.Create(wayAdderInfo.LanguageCode).ToAsync()
            from _ in current.EnsureTranslationIsNew(languageCode).ToAsync()
            from translation in WayToBeAdded.Create(
                wayAdderInfo.Id,
                new WayTranslationInput(
                    wayAdderInfo.LanguageCode,
                    wayAdderInfo.Label,
                    wayAdderInfo.Caption,
                    wayAdderInfo.Title,
                    wayAdderInfo.Description,
                    wayAdderInfo.Points
                )
            ).ToAsync()
            from __ in languageExistence.Exists(languageCode).ToAsync()
            from ___ in adder.AddTranslation(translation).ToAsync()
            select Unit.Default
        );
    }
}
