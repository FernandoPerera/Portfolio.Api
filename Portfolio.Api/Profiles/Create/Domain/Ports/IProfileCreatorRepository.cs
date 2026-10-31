using LanguageExt;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Create.Domain.Ports;

public interface IProfileCreatorRepository
{
    Task<Either<DomainError, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated);
}