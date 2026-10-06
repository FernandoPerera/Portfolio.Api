using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Find.Domain.Models;

namespace Portfolio.Api.Languages.Find.Domain.Ports;

public interface ILanguageFinderRepository
{
    Task<Either<Error, List<LanguageFound>>> FindLanguages();
}
