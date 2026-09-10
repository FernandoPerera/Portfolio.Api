using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Domain.Ports;

namespace Portfolio.Api.Profiles.Create.Application.UseCases;

public class ProfileCreator(IProfileCreatorRepository creator)
{
    public async Task<Either<Error, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
    {
        throw new NotImplementedException();
    }
}