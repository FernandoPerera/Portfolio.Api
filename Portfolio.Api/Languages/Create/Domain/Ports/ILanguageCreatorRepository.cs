using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Create.Domain.Models;

namespace Portfolio.Api.Languages.Create.Domain.Ports;

public interface ILanguageCreatorRepository
{
    Task<Either<Error, Unit>> CreateLanguage(LanguageToBeCreated languageToBeCreated);
}
