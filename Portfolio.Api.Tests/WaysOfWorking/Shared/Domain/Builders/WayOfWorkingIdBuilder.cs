using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;

public abstract class WayOfWorkingIdBuilder
{
    public static WayOfWorkingId Build(Guid? id = null)
    {
        return WayOfWorkingId.Create(id ?? Guid.NewGuid())
            .IfLeft(error => throw new Exception("Error in WayOfWorkingIdBuilder, error: " + error.Message));
    }
}
