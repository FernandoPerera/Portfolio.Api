using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Update.Domain.Models;

namespace Portfolio.Api.Languages.Update.Domain.Ports;

public interface ILanguageUpdaterRepository
{
    Task<Either<Error, Unit>> UpdateLanguage(LanguageToBeUpdated languageToBeUpdated);
}
