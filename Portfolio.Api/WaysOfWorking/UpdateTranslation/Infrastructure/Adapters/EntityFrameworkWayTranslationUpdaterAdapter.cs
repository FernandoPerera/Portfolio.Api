using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Adapters;

public class EntityFrameworkWayTranslationUpdaterAdapter(AppDbContext dbContext) : IWayTranslationUpdaterRepository
{
    public async Task<Either<DomainError, Unit>> UpdateTranslation(WayTranslationToBeUpdated translationToBeUpdated)
    {
        try
        {
            await dbContext.WayOfWorkingTranslations
                .Where(translation =>
                    translation.WayId == translationToBeUpdated.GetWayId()
                    && translation.LanguageCode == translationToBeUpdated.GetLanguageCode()
                )
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(translation => translation.Label, translationToBeUpdated.GetLabel())
                    .SetProperty(translation => translation.Caption, translationToBeUpdated.GetCaption())
                    .SetProperty(translation => translation.Title, translationToBeUpdated.GetTitle())
                    .SetProperty(translation => translation.Description, translationToBeUpdated.GetDescription())
                    .SetProperty(translation => translation.Points, translationToBeUpdated.GetPoints()));

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
