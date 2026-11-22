using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Domain.Builders;

public abstract class WayOfWorkingToBeCreatedBuilder
{
    public static WayOfWorkingToBeCreated Build(
        string illustration = "plain-language",
        List<WayTranslationInput>? translations = null
    )
    {
        return WayOfWorkingToBeCreated.Create(illustration, translations ?? [WayTranslationInputBuilder.Build()])
            .IfLeft(error => throw new Exception("Error in WayOfWorkingToBeCreatedBuilder, error: " + error.Message));
    }
}
