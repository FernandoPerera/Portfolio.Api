using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Update.Domain.Models;

namespace Portfolio.Api.Profiles.Update.Domain.Ports;

public interface IProfileUpdaterRepository
{
    Task<Either<Error, Unit>> UpdateProfile(ProfileToBeUpdated profileToBeUpdated);
}
