using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;

namespace Portfolio.Api.Timeline.Update.Infrastructure.Adapters;

public class EntityFrameworkTimelineEntryFinderAdapter(AppDbContext dbContext) : ITimelineEntryFinderRepository
{
    public async Task<Either<DomainError, TimelineEntryFound>> FindEntry(TimelineId id)
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

            return ToEntry(entity);
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }

    private static Either<DomainError, TimelineEntryFound> ToEntry(TimelineEntryEntity entity) =>
        entity.Translations
            .Select(translation => TimelineTranslation.CreateUnsafe(
                translation.LanguageCode,
                translation.Title,
                translation.Description
            ))
            .Sequence()
            .Bind(translations => TimelineEntryFound.CreateUnsafe(
                entity.Id,
                entity.Organization,
                entity.Category,
                entity.StartDate,
                entity.EndDate,
                entity.Technologies,
                translations.ToList()
            ));
}
