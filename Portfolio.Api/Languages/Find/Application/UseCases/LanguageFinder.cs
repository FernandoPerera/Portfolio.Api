using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Languages.Find.Domain.Ports;

namespace Portfolio.Api.Languages.Find.Application.UseCases;

public class LanguageFinder(ILanguageFinderRepository repository)
{
    public async Task<Either<Error, List<LanguageFound>>> Execute()
    {
        return await (
            from languages in repository.FindLanguages().ToAsync()
            select languages
        );
    }
}
