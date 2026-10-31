using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;

public class EntityFrameworkLanguageExistenceAdapter(AppDbContext dbContext) : ILanguageExistenceRepository
{
    public async Task<Either<DomainError, Unit>> Exists(LanguageCode languageCode)
    {
        try
        {
            var code = languageCode.ToString();
            var exists = await dbContext.Languages.AnyAsync(l => l.LanguageCode == code);
            if (!exists)
            {
                return new NotFoundError($"Language '{code}' was not found");
            }

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
