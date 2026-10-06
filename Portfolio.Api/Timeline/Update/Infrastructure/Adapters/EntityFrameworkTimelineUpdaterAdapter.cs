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

            timelineEntryToBeUpdated.GetOrganization().IfSome(organization => entity.Organization = organization);
            timelineEntryToBeUpdated.GetCategory().IfSome(category => entity.Category = category);
            timelineEntryToBeUpdated.GetStartDate().IfSome(startDate => entity.StartDate = startDate);
            timelineEntryToBeUpdated.GetEndDate().IfSome(endDate => entity.EndDate = endDate);
            timelineEntryToBeUpdated.GetTechnologies().IfSome(technologies => entity.Technologies = technologies);
            timelineEntryToBeUpdated.GetTranslations().IfSome(translations => ReplaceTranslations(entity, translations));

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }

    private void ReplaceTranslations(TimelineEntryEntity entity, List<TimelineTranslation> translations)
    {
        var givenLanguages = translations.Select(translation => translation.GetLanguageCode()).ToList();

        var removed = entity.Translations
            .Where(translation => !givenLanguages.Contains(translation.LanguageCode))
            .ToList();
        dbContext.TimelineEntryTranslations.RemoveRange(removed);

        foreach (var translation in translations)
        {
            var existing = entity.Translations
                .SingleOrDefault(current => current.LanguageCode == translation.GetLanguageCode());

            if (existing is null)
            {
                entity.Translations.Add(new TimelineEntryTranslationEntity
                {
                    EntryId = entity.Id,
                    LanguageCode = translation.GetLanguageCode(),
                    Title = translation.GetTitle(),
                    Description = translation.GetDescription()
                });
                continue;
            }

            existing.Title = translation.GetTitle();
            existing.Description = translation.GetDescription();
        }
    }
}
