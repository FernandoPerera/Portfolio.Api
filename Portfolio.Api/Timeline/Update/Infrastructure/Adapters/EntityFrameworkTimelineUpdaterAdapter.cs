using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;

namespace Portfolio.Api.Timeline.Update.Infrastructure.Adapters;

public class EntityFrameworkTimelineUpdaterAdapter(AppDbContext dbContext) : ITimelineUpdaterRepository
{
    public async Task<Either<Error, Unit>> UpdateEntry(TimelineEntryToBeUpdated timelineEntryToBeUpdated)
    {
        try
        {
            var id = timelineEntryToBeUpdated.GetId();
            var entity = await dbContext.TimelineEntries
                .Include(entry => entry.Translations)
                .SingleOrDefaultAsync(entry => entry.Id == id);

            if (entity is null)
            {
                return Error.New(new NotFoundException($"Timeline entry '{id}' was not found"));
            }

            var languageCode = timelineEntryToBeUpdated.GetLanguageCode();
            var translation = entity.Translations.SingleOrDefault(current => current.LanguageCode == languageCode);
            if (translation is null)
            {
                return Error.New(new NotFoundException($"Timeline translation for language '{languageCode}' was not found"));
            }

            entity.Organization = timelineEntryToBeUpdated.GetOrganization();
            entity.Category = timelineEntryToBeUpdated.GetCategory();
            entity.StartDate = timelineEntryToBeUpdated.GetStartDate();
            entity.EndDate = timelineEntryToBeUpdated.GetEndDate().ToNullable();
            entity.Technologies = timelineEntryToBeUpdated.GetTechnologies();
            translation.Title = timelineEntryToBeUpdated.GetTitle();
            translation.Description = timelineEntryToBeUpdated.GetDescription();

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
