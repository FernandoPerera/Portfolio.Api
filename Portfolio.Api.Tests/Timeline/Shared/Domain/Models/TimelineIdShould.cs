using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Models;

public class TimelineIdShould
{
    [Fact]
    public void BeCreated()
    {
        var givenId = Guid.NewGuid();

        TimelineId.Create(givenId)
            .Match(
                timelineId => timelineId.ToGuid().ShouldBe(givenId),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void NotAllowEmptyValue()
    {
        TimelineId.Create(Guid.Empty)
            .Match(
                timelineId => timelineId.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Timeline id cannot be empty")
            );
    }
}
