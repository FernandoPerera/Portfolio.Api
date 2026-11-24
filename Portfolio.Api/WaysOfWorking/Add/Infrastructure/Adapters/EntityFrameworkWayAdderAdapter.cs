using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Add.Domain.Models;
using Portfolio.Api.WaysOfWorking.Add.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Add.Infrastructure.Adapters;

public class EntityFrameworkWayAdderAdapter(AppDbContext dbContext) : IWayAdderRepository
{
    public async Task<Either<DomainError, Unit>> AddTranslation(WayToBeAdded toBeAdded)
    {
        try
        {
            var translation = toBeAdded.GetTranslation();
            await dbContext.WayOfWorkingTranslations.AddAsync(new WayOfWorkingTranslationEntity
            {
                WayId = toBeAdded.GetWayId(),
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
