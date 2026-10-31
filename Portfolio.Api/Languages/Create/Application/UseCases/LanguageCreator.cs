using LanguageExt;
using Portfolio.Api.Languages.Create.Application.Dtos;
using Portfolio.Api.Languages.Create.Domain.Models;
using Portfolio.Api.Languages.Create.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Languages.Create.Application.UseCases;

public class LanguageCreator(ILanguageCreatorRepository creator)
{
    public async Task<Either<DomainError, Unit>> Execute(LanguageCreatorInfo languageCreatorInfo)
    {
        return await (
            from language in LanguageToBeCreated.Create(
                languageCreatorInfo.LanguageCode,
                languageCreatorInfo.Name
            ).ToAsync()
            from result in creator.CreateLanguage(language).ToAsync()
            select result
        );
    }
}
