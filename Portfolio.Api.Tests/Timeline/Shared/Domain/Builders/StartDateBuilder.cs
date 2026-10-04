using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;

public abstract class StartDateBuilder
{
    public static StartDate Build(string startDate = "2023-01")
    {
        return StartDate.Create(startDate)
            .IfLeft(error => throw new Exception("Error in StartDateBuilder, error: " + error.Message));
    }
}
