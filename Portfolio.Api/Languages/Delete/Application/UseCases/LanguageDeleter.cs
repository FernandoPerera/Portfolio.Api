using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Languages.Delete.Application.Dtos;
using Portfolio.Api.Languages.Delete.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;

namespace Portfolio.Api.Languages.Delete.Application.UseCases;

public class LanguageDeleter(
    ILanguageExistenceRepository languageExistence,
    ILanguageUsageRepository languageUsage,
    ILanguageDeleterRepository deleter
)
{
    public async Task<Either<Error, Unit>> Execute(LanguageDeleterInfo languageDeleterInfo)
    {
        return await (
            from languageCode in LanguageCode.Create(languageDeleterInfo.LanguageCode).ToAsync()
            from _ in languageExistence.Exists(languageCode).ToAsync()
            from __ in languageUsage.EnsureIsNotInUse(languageCode).ToAsync()
            from result in deleter.DeleteLanguage(languageCode).ToAsync()
            select result
        );
    }
}
