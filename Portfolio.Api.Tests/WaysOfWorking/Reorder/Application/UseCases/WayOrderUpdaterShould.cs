using LanguageExt;
using NSubstitute;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Reorder.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Reorder.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Reorder.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Reorder.Application.UseCases;

public class WayOrderUpdaterShould
{
    private readonly IWayOfWorkingFinderRepository _finderRepository = Substitute.For<IWayOfWorkingFinderRepository>();
    private readonly IWayOrderRepository _orderRepository = Substitute.For<IWayOrderRepository>();
    private readonly WayOrderUpdater _updater;

    public WayOrderUpdaterShould()
    {
        _updater = new WayOrderUpdater(_finderRepository, _orderRepository);
        _orderRepository.ReorderWays(Arg.Any<WayOrder>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task ReorderTheWaysOfWorking()
    {
        var first = WayOfWorkingFoundBuilder.Build(position: 1);
        var second = WayOfWorkingFoundBuilder.Build(position: 2);
        _finderRepository.FindWays().Returns(new List<WayOfWorkingFound> { first, second });
        var givenIds = new List<Guid> { second.GetId(), first.GetId() };

        var result = await _updater.Execute(new WayOrderUpdaterInfo(givenIds));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _orderRepository.Received(1).ReorderWays(Arg.Is<WayOrder>(order => order.GetValues().SequenceEqual(givenIds)));
            },
            error => error.ShouldBeNull()
        );
    }
}
