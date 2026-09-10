using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;

namespace Portfolio.Api.Profiles.Create.Application.UseCases;

public class ProfileCreator(IProfileCreatorRepository creator)
{
    public async Task<Either<Error, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
    {
        return await (
            from profile in ProfileToBeCreated.Create(
                profileCreatorInfo.Name,
                profileCreatorInfo.Presentation,
                profileCreatorInfo.Role,
                profileCreatorInfo.Enterprise
            ).ToAsync()
            from _ in creator.CreateProfile(profile).ToAsync()
            select Unit.Default
        );
    }
}