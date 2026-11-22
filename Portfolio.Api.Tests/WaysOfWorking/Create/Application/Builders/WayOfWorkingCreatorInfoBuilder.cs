using Portfolio.Api.WaysOfWorking.Create.Application.Dtos;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Application.Builders;

public abstract class WayOfWorkingCreatorInfoBuilder
{
    public static WayOfWorkingCreatorInfo Build(
        string illustration = "plain-language",
        List<WayTranslationInfo>? translations = null
    )
    {
        return new WayOfWorkingCreatorInfo(
            illustration,
            translations ?? [WayTranslationInfoBuilder.Build()]
        );
    }
}
