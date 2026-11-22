using Portfolio.Api.Shared.Domain.Errors;
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
}
