using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Delete.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingDeleterAdapter(AppDbContext dbContext) : IWayOfWorkingDeleterRepository
{
    public async Task<Either<DomainError, Unit>> DeleteWay(WayOfWorkingId id)
    {
        try
        {
            var ways = await dbContext.WaysOfWorking.OrderBy(way => way.Position).ToListAsync();

            dbContext.WaysOfWorking.RemoveRange(ways.Where(way => way.Id == id.ToGuid()));
            AssignConsecutivePositions(ways.Where(way => way.Id != id.ToGuid()).ToList());

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }

    private static void AssignConsecutivePositions(List<WayOfWorkingEntity> ways)
    {
        for (var index = 0; index < ways.Count; index++)
        {
            ways[index].Position = index + 1;
        }
    }
}
