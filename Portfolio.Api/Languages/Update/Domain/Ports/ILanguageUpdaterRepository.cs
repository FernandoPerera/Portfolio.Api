using LanguageExt;
using Portfolio.Api.Languages.Update.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Update.Domain.Ports;

public interface ILanguageUpdaterRepository
{
    Task<Either<DomainError, Unit>> UpdateLanguage(LanguageToBeUpdated languageToBeUpdated);
}
