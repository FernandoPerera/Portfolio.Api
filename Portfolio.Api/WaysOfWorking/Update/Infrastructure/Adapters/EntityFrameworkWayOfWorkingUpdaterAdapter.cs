using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Update.Domain.Models;
using Portfolio.Api.WaysOfWorking.Update.Domain.Ports;

namespace Portfolio.Api.WaysOfWorking.Update.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingUpdaterAdapter(AppDbContext dbContext) : IWayOfWorkingUpdaterRepository
{
    public async Task<Either<DomainError, Unit>> UpdateWay(WayOfWorkingToBeUpdated wayOfWorkingToBeUpdated)
    {
        try
        {
            await dbContext.WaysOfWorking
                .Where(way => way.Id == wayOfWorkingToBeUpdated.GetId())
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(way => way.Illustration, wayOfWorkingToBeUpdated.GetIllustration()));

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
