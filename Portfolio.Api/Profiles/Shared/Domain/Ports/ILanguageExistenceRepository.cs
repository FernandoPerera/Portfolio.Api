using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;

namespace Portfolio.Api.Profiles.Shared.Domain.Ports;

public interface ILanguageExistenceRepository
{
    Task<Either<Error, Unit>> Exists(LanguageCode languageCode);
}
