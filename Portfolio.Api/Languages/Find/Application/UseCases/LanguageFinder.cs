using LanguageExt;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Languages.Find.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Find.Application.UseCases;

public class LanguageFinder(ILanguageFinderRepository repository)
{
    public async Task<Either<DomainError, List<LanguageFound>>> Execute()
    {
        return await (
            from languages in repository.FindLanguages().ToAsync()
            select languages
        );
    }
}
