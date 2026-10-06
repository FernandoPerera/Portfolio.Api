using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;

public static class TimelineIdBuilder
{
    public static TimelineId Build(Guid? id = null)
    {
        return TimelineId.Create(id ?? Guid.NewGuid())
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(TimelineId)} in tests: {error}"));
    }
}
