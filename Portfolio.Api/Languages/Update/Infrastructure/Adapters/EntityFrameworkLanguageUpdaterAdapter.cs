using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Update.Domain.Models;
using Portfolio.Api.Languages.Update.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Languages.Update.Infrastructure.Adapters;

public class EntityFrameworkLanguageUpdaterAdapter(AppDbContext dbContext) : ILanguageUpdaterRepository
{
    public async Task<Either<DomainError, Unit>> UpdateLanguage(LanguageToBeUpdated languageToBeUpdated)
    {
        try
        {
            var languageCode = languageToBeUpdated.GetLanguageCode();
            var entity = await dbContext.Languages.SingleOrDefaultAsync(language => language.LanguageCode == languageCode);
            if (entity is null)
            {
                return new NotFoundError($"Language '{languageCode}' was not found");
            }

            entity.Name = languageToBeUpdated.GetName();

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
