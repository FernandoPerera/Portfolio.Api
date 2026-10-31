using LanguageExt;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Update.Domain.Ports;

public interface IProfileUpdaterRepository
{
    Task<Either<DomainError, Unit>> UpdateProfile(ProfileToBeUpdated profileToBeUpdated);
}
