using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Delete.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Languages.Delete.Infrastructure.Adapters;

public class EntityFrameworkLanguageUsageAdapter(AppDbContext dbContext) : ILanguageUsageRepository
{
    public async Task<Either<DomainError, Unit>> EnsureIsNotInUse(LanguageCode languageCode)
    {
        try
        {
            var code = languageCode.ToString();
            var isInUse = await dbContext.ProfileTranslations.AnyAsync(translation => translation.LanguageCode == code)
                          || await dbContext.TimelineEntryTranslations.AnyAsync(translation => translation.LanguageCode == code);
            if (isInUse)
            {
                return new ConflictError($"Language '{code}' is used by existing translations");
            }

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
