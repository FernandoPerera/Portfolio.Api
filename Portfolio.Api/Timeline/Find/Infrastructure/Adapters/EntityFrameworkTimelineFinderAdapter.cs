using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Adapters;

public class EntityFrameworkTimelineFinderAdapter(AppDbContext dbContext) : ITimelineFinderRepository
{
    public async Task<Either<DomainError, List<TimelineEntryFound>>> FindEntries()
    {
        try
        {
            var entities = await dbContext.TimelineEntries
                .Include(entry => entry.Translations)
                .OrderByDescending(entry => entry.StartDate)
                .ToListAsync();

            return entities
                .Select(ToEntry)
                .Sequence()
                .Map(entries => entries.ToList());
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }

    private static Either<DomainError, TimelineEntryFound> ToEntry(TimelineEntryEntity entity) =>
        entity.Translations
            .Select(ToTranslation)
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

    private static Either<DomainError, TimelineTranslation> ToTranslation(TimelineEntryTranslationEntity entity) =>
        TimelineTranslation.CreateUnsafe(entity.LanguageCode, entity.Title, entity.Description);
}
