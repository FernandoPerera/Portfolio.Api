using Portfolio.Api.Timeline.Create.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Builders;

public abstract class EndDateBuilder
{
    public static EndDate Build(string? endDate = null)
    {
        return EndDate.Create(endDate)
            .IfLeft(error => throw new Exception("Error in EndDateBuilder, error: " + error.Message));
    }
}
