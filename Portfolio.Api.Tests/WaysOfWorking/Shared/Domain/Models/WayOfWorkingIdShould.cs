using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayOfWorkingIdShould
{
    [Fact]
    public void BeCreated()
    {
        var givenId = Guid.NewGuid();

        WayOfWorkingId.Create(givenId)
            .Match(
                wayId => wayId.ToGuid().ShouldBe(givenId),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void NotAllowEmptyValue()
    {
        WayOfWorkingId.Create(Guid.Empty)
            .Match(
                wayId => wayId.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Way of working id cannot be empty")
            );
    }
}
