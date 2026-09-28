using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;

public class EntityFrameworkLanguageExistenceAdapter(AppDbContext dbContext) : ILanguageExistenceRepository
{
    public async Task<Either<Error, Unit>> Exists(LanguageCode languageCode)
    {
        try
        {
            var code = languageCode.ToString();
            var exists = await dbContext.Languages.AnyAsync(l => l.LanguageCode == code);
            if (!exists)
            {
                return Error.New(new NotFoundException($"Language '{code}' was not found"));
            }

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
