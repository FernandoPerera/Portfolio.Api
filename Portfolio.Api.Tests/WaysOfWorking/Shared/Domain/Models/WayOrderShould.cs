using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayOrderShould
{
    [Fact]
    public void BeCreated()
    {
        var givenIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        WayOrder.Create(givenIds)
            .Match(
                order => order.GetValues().ShouldBe(givenIds),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void GiveThePositionOfEachWayOfWorking()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        WayOrder.Create([second, first])
            .Match(
                order =>
                {
                    order.GetPositionOf(second).ShouldBe(1);
                    order.GetPositionOf(first).ShouldBe(2);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectAnOrderWithoutWaysOfWorking()
    {
        WayOrder.Create([])
            .Match(
                order => order.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The order needs at least one way of working")
            );
    }

    [Fact]
    public void RejectAnOrderWhenAWayOfWorkingIsRepeated()
    {
        var repeated = Guid.NewGuid();

        WayOrder.Create([repeated, Guid.NewGuid(), repeated])
            .Match(
                order => order.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The order cannot repeat a way of working")
            );
    }

    [Fact]
    public void CoverTheExistingWaysOfWorkingInAnyOrder()
    {
        var first = WayOfWorkingFoundBuilder.Build();
        var second = WayOfWorkingFoundBuilder.Build(position: 2);

        WayOrder.Create([second.GetId(), first.GetId()])
            .Bind(order => order.EnsureCoversExactly([first, second]))
            .Match(
                unit => unit.ShouldBeUnit(),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectAnOrderThatLeavesAWayOfWorkingOut()
    {
        var first = WayOfWorkingFoundBuilder.Build();
        var second = WayOfWorkingFoundBuilder.Build(position: 2);

        WayOrder.Create([first.GetId()])
            .Bind(order => order.EnsureCoversExactly([first, second]))
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The order must contain exactly the existing ways of working")
            );
    }

    [Fact]
    public void RejectAnOrderWithAWayOfWorkingThatDoesNotExist()
    {
        var first = WayOfWorkingFoundBuilder.Build();

        WayOrder.Create([first.GetId(), Guid.NewGuid()])
            .Bind(order => order.EnsureCoversExactly([first]))
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("The order must contain exactly the existing ways of working")
            );
    }
}
