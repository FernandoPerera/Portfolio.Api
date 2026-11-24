using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Reorder.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Adapters;

public class EntityFrameworkWayOrderAdapter(AppDbContext dbContext) : IWayOrderRepository
{
    public async Task<Either<DomainError, Unit>> ReorderWays(WayOrder order)
    {
        try
        {
            var ways = await dbContext.WaysOfWorking.ToListAsync();
            foreach (var way in ways)
            {
                way.Position = order.GetPositionOf(way.Id);
            }

            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
