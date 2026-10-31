using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Shared.Domain.Ports;

public interface ILanguageExistenceRepository
{
    Task<Either<DomainError, Unit>> Exists(LanguageCode languageCode);
}
