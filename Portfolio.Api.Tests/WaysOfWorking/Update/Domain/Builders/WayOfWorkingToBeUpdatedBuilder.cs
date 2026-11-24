using Portfolio.Api.WaysOfWorking.Update.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.Update.Domain.Builders;

public abstract class WayOfWorkingToBeUpdatedBuilder
{
    public static WayOfWorkingToBeUpdated Build(Guid? id = null, string illustration = "fast-response")
    {
        return WayOfWorkingToBeUpdated.Create(id ?? Guid.NewGuid(), illustration)
            .IfLeft(error => throw new Exception("Error in WayOfWorkingToBeUpdatedBuilder, error: " + error.Message));
    }
}
