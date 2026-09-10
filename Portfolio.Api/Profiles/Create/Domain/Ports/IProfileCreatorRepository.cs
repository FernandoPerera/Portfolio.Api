using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Domain.Models;

namespace Portfolio.Api.Profiles.Create.Domain.Ports;

public interface IProfileCreatorRepository
{
    Task<Either<Error, Unit>> CreateProfile(Profile profile);
}