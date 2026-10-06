using LanguageExt;
using LanguageExt.Common;
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
    public async Task<Either<Error, TimelineEntryFound>> FindEntry(Guid id)
    {
        try
        {
            var entity = await dbContext.TimelineEntries
                .Include(entry => entry.Translations)
                .SingleOrDefaultAsync(entry => entry.Id == id);

            if (entity is null)
            {
                return Error.New(new NotFoundException($"Timeline entry '{id}' was not found"));
            }

            return ToEntry(entity);
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }

    private static Either<Error, TimelineEntryFound> ToEntry(TimelineEntryEntity entity) =>
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
