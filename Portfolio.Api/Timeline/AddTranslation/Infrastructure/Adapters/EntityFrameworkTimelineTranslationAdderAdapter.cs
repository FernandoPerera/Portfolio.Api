using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Portfolio.Api.Timeline.AddTranslation.Domain.Ports;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;

namespace Portfolio.Api.Timeline.AddTranslation.Infrastructure.Adapters;

public class EntityFrameworkTimelineTranslationAdderAdapter(AppDbContext dbContext) : ITimelineTranslationAdderRepository
{
    public async Task<Either<Error, Unit>> AddTranslation(TimelineTranslationToBeAdded translationToBeAdded)
    {
        try
        {
            await dbContext.TimelineEntryTranslations.AddAsync(new TimelineEntryTranslationEntity
            {
                EntryId = translationToBeAdded.GetEntryId(),
                LanguageCode = translationToBeAdded.GetLanguageCode(),
                Title = translationToBeAdded.GetTitle(),
                Description = translationToBeAdded.GetDescription()
            });
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
