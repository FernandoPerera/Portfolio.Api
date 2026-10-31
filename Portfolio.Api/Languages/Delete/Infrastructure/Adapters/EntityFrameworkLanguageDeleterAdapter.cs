using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Delete.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Languages.Delete.Infrastructure.Adapters;

public class EntityFrameworkLanguageDeleterAdapter(AppDbContext dbContext) : ILanguageDeleterRepository
{
    public async Task<Either<Error, Unit>> DeleteLanguage(LanguageCode languageCode)
    {
        try
        {
            var code = languageCode.ToString();
            var entity = await dbContext.Languages.SingleOrDefaultAsync(language => language.LanguageCode == code);
            if (entity is null)
            {
                return Error.New(new NotFoundException($"Language '{code}' was not found"));
            }

            dbContext.Languages.Remove(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
