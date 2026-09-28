using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Ports;

namespace Portfolio.Api.Profiles.Create.Application.UseCases;

public class ProfileCreator(
    IProfileCreatorRepository creator,
    ILanguageFinderRepository languageFinder
)
{
    public async Task<Either<Error, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
    {
        return await (
            from profile in ProfileToBeCreated.Create(
                profileCreatorInfo.Name,
                profileCreatorInfo.Presentation,
                profileCreatorInfo.JobRole,
                profileCreatorInfo.EnterpriseName,
                profileCreatorInfo.LanguageCode
            ).ToAsync()
            from _ in languageFinder.Find(profile.GetLanguageCodeModel()).ToAsync()
            from result in creator.CreateProfile(profile).ToAsync()
            select result
        );
    }
}