using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Portfolio.Api.Timeline.Create.Domain.Ports;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Adapters;

public class EntityFrameworkTimelineCreatorAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier
) : ITimelineCreatorRepository
{
    public async Task<Either<Error, Unit>> CreateEntry(TimelineEntryToBeCreated timelineEntryToBeCreated)
    {
        try
        {
            var entryId = generatorIdentifier.Generate();
            var entity = new TimelineEntryEntity
            {
                Id = entryId,
                Organization = timelineEntryToBeCreated.GetOrganization(),
                Category = timelineEntryToBeCreated.GetCategory(),
                StartDate = timelineEntryToBeCreated.GetStartDate(),
                EndDate = timelineEntryToBeCreated.GetEndDate().ToNullable(),
                Technologies = timelineEntryToBeCreated.GetTechnologies(),
                Translations = timelineEntryToBeCreated.GetTranslations()
                    .Select(translation => new TimelineEntryTranslationEntity
                    {
                        EntryId = entryId,
                        LanguageCode = translation.GetLanguageCode(),
                        Title = translation.GetTitle(),
                        Description = translation.GetDescription()
                    })
                    .ToList()
            };

            await dbContext.TimelineEntries.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
