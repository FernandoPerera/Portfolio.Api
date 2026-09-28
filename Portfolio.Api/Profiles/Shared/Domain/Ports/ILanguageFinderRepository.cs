using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;

namespace Portfolio.Api.Profiles.Shared.Domain.Ports;

public interface ILanguageFinderRepository
{
    Task<Either<Error, Unit>> Find(LanguageCode languageCode);
}
