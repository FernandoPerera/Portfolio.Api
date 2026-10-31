using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Delete.Domain.Ports;

public interface ILanguageUsageRepository
{
    Task<Either<DomainError, Unit>> EnsureIsNotInUse(LanguageCode languageCode);
}
