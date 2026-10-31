using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Delete.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Delete.Infrastructure.Adapters;

public class EntityFrameworkTimelineDeleterAdapter(AppDbContext dbContext) : ITimelineDeleterRepository
{
    public async Task<Either<DomainError, Unit>> DeleteEntry(TimelineId id)
    {
        try
        {
            var entryId = id.ToGuid();
            var entity = await dbContext.TimelineEntries
                .Include(entry => entry.Translations)
                .SingleOrDefaultAsync(entry => entry.Id == entryId);
            if (entity is null)
            {
                return new NotFoundError($"Timeline entry '{entryId}' was not found");
            }

            dbContext.TimelineEntryTranslations.RemoveRange(entity.Translations);
            dbContext.TimelineEntries.Remove(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
