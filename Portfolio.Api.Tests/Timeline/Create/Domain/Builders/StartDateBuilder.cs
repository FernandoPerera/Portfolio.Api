using Portfolio.Api.Timeline.Create.Domain.Models;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Builders;

public abstract class StartDateBuilder
{
    public static StartDate Build(string startDate = "2023-01")
    {
        return StartDate.Create(startDate)
            .IfLeft(error => throw new Exception("Error in StartDateBuilder, error: " + error.Message));
    }
}
