using LanguageExt;
using Portfolio.Api.Languages.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Create.Domain.Ports;

public interface ILanguageCreatorRepository
{
    Task<Either<DomainError, Unit>> CreateLanguage(LanguageToBeCreated languageToBeCreated);
}
