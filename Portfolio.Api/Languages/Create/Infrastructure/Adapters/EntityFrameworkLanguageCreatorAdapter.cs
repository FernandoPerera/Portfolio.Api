using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Create.Domain.Models;
using Portfolio.Api.Languages.Create.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Languages.Create.Infrastructure.Adapters;

public class EntityFrameworkLanguageCreatorAdapter(AppDbContext dbContext) : ILanguageCreatorRepository
{
    public async Task<Either<Error, Unit>> CreateLanguage(LanguageToBeCreated languageToBeCreated)
    {
        try
        {
            var code = languageToBeCreated.GetLanguageCode();
            var exists = await dbContext.Languages.AnyAsync(l => l.LanguageCode == code);
            if (exists)
            {
                return Unit.Default;
            }

            await dbContext.Languages.AddAsync(new LanguageEntity
            {
                LanguageCode = code,
                Name = languageToBeCreated.GetName()
            });
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
