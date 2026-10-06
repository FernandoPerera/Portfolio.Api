using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Update.Application.Dtos;
using Portfolio.Api.Languages.Update.Domain.Models;
using Portfolio.Api.Languages.Update.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Ports;

namespace Portfolio.Api.Languages.Update.Application.UseCases;

public class LanguageUpdater(
    ILanguageExistenceRepository languageExistence,
    ILanguageUpdaterRepository updater
)
{
    public async Task<Either<Error, Unit>> Execute(LanguageUpdaterInfo languageUpdaterInfo)
    {
        return await (
            from language in LanguageToBeUpdated.Create(
                languageUpdaterInfo.LanguageCode,
                languageUpdaterInfo.Name
            ).ToAsync()
            from _ in languageExistence.Exists(language.GetLanguageCodeModel()).ToAsync()
            from result in updater.UpdateLanguage(language).ToAsync()
            select result
        );
    }
}
