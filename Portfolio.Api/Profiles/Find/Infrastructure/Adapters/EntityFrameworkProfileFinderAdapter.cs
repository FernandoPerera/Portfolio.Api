using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Adapters;

public class EntityFrameworkProfileFinderAdapter(AppDbContext dbContext) : IProfileFinderRepository
{
    public async Task<Either<Error, ProfileFound>> FindProfile()
    {
        try
        {
            var entity = await dbContext.Profiles.Include(p => p.Translations).FirstOrDefaultAsync();
            if (entity is null)
            {
                return Error.New(new NotFoundException("Profile was not found"));
            }

            return (
                from translations in entity.Translations
                    .OrderBy(translation => translation.LanguageCode)
                    .Select(translation => ProfileTranslationFound.CreateUnsafe(
                        translation.LanguageCode,
                        translation.Presentation,
                        translation.Role
                    ))
                    .Sequence()
                from profile in ProfileFound.CreateUnsafe(
                    entity.Name,
                    entity.Enterprise,
                    translations.ToList()
                )
                select profile
            );
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
