using LanguageExt;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Create.Application.UseCases;

public class ProfileCreator(
    IProfileCreatorRepository creator,
    ILanguageExistenceRepository languageExistence
)
{
    public async Task<Either<DomainError, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
    {
        return await (
            from profile in ProfileToBeCreated.Create(
                profileCreatorInfo.Name,
                profileCreatorInfo.Presentation,
                profileCreatorInfo.JobRole,
                profileCreatorInfo.EnterpriseName,
                profileCreatorInfo.LanguageCode
            ).ToAsync()
            from _ in languageExistence.Exists(profile.GetLanguageCodeModel()).ToAsync()
            from result in creator.CreateProfile(profile).ToAsync()
            select result
        );
    }
}