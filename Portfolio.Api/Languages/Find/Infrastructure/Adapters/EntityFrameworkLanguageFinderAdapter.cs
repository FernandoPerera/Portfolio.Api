using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Languages.Find.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Languages.Find.Infrastructure.Adapters;

public class EntityFrameworkLanguageFinderAdapter(AppDbContext dbContext) : ILanguageFinderRepository
{
    public async Task<Either<Error, List<LanguageFound>>> FindLanguages()
    {
        try
        {
            var entities = await dbContext.Languages
                .OrderBy(language => language.LanguageCode)
                .ToListAsync();

            return entities
                .Select(ToLanguage)
                .Sequence()
                .Map(languages => languages.ToList());
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }

    private static Either<Error, LanguageFound> ToLanguage(LanguageEntity entity) =>
        LanguageFound.CreateUnsafe(entity.LanguageCode, entity.Name);
}
