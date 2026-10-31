using LanguageExt;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Find.Domain.Ports;

public interface ILanguageFinderRepository
{
    Task<Either<DomainError, List<LanguageFound>>> FindLanguages();
}
