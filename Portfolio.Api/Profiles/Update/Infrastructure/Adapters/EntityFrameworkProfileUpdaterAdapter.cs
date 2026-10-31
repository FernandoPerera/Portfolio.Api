using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Profiles.Update.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Update.Infrastructure.Adapters;

public class EntityFrameworkProfileUpdaterAdapter(AppDbContext dbContext) : IProfileUpdaterRepository
{
    public async Task<Either<Error, Unit>> UpdateProfile(ProfileToBeUpdated profileToBeUpdated)
    {
        try
        {
            var entity = await dbContext.Profiles.Include(p => p.Translations).FirstOrDefaultAsync();
            if (entity is null)
            {
                return Error.New(new NotFoundException("Profile was not found"));
            }

            var languageCode = profileToBeUpdated.GetLanguageCode();
            var translation = entity.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (translation is null)
            {
                return Error.New(new NotFoundException($"Profile translation for language '{languageCode}' was not found"));
            }

            entity.Name = profileToBeUpdated.GetName();
            entity.Enterprise = profileToBeUpdated.GetEnterprise();
            translation.Role = profileToBeUpdated.GetRole();
            translation.Presentation = profileToBeUpdated.GetPresentation();

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
