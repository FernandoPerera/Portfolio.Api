using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;

namespace Portfolio.Api.Languages.Delete.Domain.Ports;

public interface ILanguageUsageRepository
{
    Task<Either<Error, Unit>> EnsureIsNotInUse(LanguageCode languageCode);
}
