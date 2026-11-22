using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Adapters;

public class EntityFrameworkWayTranslationAdderAdapter(AppDbContext dbContext) : IWayTranslationAdderRepository
{
    public async Task<Either<DomainError, Unit>> AddTranslation(WayTranslationToBeAdded translationToBeAdded)
    {
        try
        {
            var translation = translationToBeAdded.GetTranslation();
            await dbContext.WayOfWorkingTranslations.AddAsync(new WayOfWorkingTranslationEntity
            {
                WayId = translationToBeAdded.GetWayId(),
                LanguageCode = translation.GetLanguageCode(),
                Label = translation.GetLabel(),
                Caption = translation.GetCaption(),
                Title = translation.GetTitle(),
                Description = translation.GetDescription(),
                Points = translation.GetPoints()
            });
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
