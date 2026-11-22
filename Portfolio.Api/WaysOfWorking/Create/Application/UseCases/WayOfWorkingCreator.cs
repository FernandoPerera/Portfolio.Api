using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Create.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;
using Portfolio.Api.WaysOfWorking.Create.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.WaysOfWorking.Create.Application.UseCases;

public class WayOfWorkingCreator(
    IWayOfWorkingCreatorRepository creator,
    ILanguageExistenceRepository languageExistence
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayOfWorkingCreatorInfo wayOfWorkingCreatorInfo)
    {
        return await (
            from way in WayOfWorkingToBeCreated.Create(
                wayOfWorkingCreatorInfo.Illustration,
                wayOfWorkingCreatorInfo.Translations
                    .Select(translation => new WayTranslationInput(
                        translation.LanguageCode,
                        translation.Label,
                        translation.Caption,
                        translation.Title,
                        translation.Description,
                        translation.Points
                    ))
                    .ToList()
            ).ToAsync()
            from _ in EnsureLanguagesExist(way).ToAsync()
            from result in creator.CreateWay(way).ToAsync()
            select result
        );
    }

    private async Task<Either<DomainError, Unit>> EnsureLanguagesExist(WayOfWorkingToBeCreated way)
    {
        foreach (var translation in way.GetTranslations())
        {
            var exists = await languageExistence.Exists(translation.GetLanguageCodeModel());
            if (exists.IsLeft)
            {
                return exists;
            }
        }

        return Unit.Default;
    }
}
