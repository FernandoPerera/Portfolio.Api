using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Reorder.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Reorder.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Reorder.Application.UseCases;

public class WayOrderUpdater(
    IWayOfWorkingFinderRepository finder,
    IWayOrderRepository orderRepository
)
{
    public async Task<Either<DomainError, Unit>> Execute(WayOrderUpdaterInfo wayOrderUpdaterInfo)
    {
        return await (
            from order in WayOrder.Create(wayOrderUpdaterInfo.Ids).ToAsync()
            from existingWays in finder.FindWays().ToAsync()
            from _ in order.EnsureCoversExactly(existingWays).ToAsync()
            from result in orderRepository.ReorderWays(order).ToAsync()
            select result
        );
    }
}
