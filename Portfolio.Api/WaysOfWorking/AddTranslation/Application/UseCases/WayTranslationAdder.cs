using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.AddTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Application.UseCases;

public class WayTranslationAdder(
    IWayOfWorkingByIdFinderRepository finder,
    ILanguageExistenceRepository languageExistence,
    IWayTranslationAdderRepository adder
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayTranslationAdderInfo wayTranslationAdderInfo)
    {
        return await (
            from id in WayOfWorkingId.Create(wayTranslationAdderInfo.Id).ToAsync()
            from current in finder.FindWay(id).ToAsync()
            from languageCode in LanguageCode.Create(wayTranslationAdderInfo.LanguageCode).ToAsync()
            from _ in current.EnsureTranslationIsNew(languageCode).ToAsync()
            from translation in WayTranslationToBeAdded.Create(
                wayTranslationAdderInfo.Id,
                new WayTranslationInput(
                    wayTranslationAdderInfo.LanguageCode,
                    wayTranslationAdderInfo.Label,
                    wayTranslationAdderInfo.Caption,
                    wayTranslationAdderInfo.Title,
                    wayTranslationAdderInfo.Description,
                    wayTranslationAdderInfo.Points
                )
            ).ToAsync()
            from __ in languageExistence.Exists(languageCode).ToAsync()
            from ___ in adder.AddTranslation(translation).ToAsync()
            select Unit.Default
        );
    }
}
