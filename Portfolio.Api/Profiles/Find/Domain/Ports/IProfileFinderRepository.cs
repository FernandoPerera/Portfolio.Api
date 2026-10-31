using LanguageExt;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Domain.Ports;

public interface IProfileFinderRepository
{
    Task<Either<DomainError, ProfileFound>> FindProfile();
}
