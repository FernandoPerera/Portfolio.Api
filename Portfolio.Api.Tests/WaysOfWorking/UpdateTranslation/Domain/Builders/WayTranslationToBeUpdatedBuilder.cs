using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Domain.Builders;

public abstract class WayTranslationToBeUpdatedBuilder
{
    public static WayTranslationToBeUpdated Build(
        Guid? wayId = null,
        string languageCode = "es",
        string? label = "Claro y simple",
        string? caption = null,
        string? title = null,
        string? description = null,
        List<string>? points = null
    )
    {
        var current = WayOfWorkingFoundBuilder.Build(
            wayId,
            translations: [WayTranslationBuilder.Build(languageCode)]
        );

        return WayTranslationToBeUpdated.Create(current, languageCode, label, caption, title, description, points)
            .IfLeft(error => throw new Exception("Error in WayTranslationToBeUpdatedBuilder, error: " + error.Message));
    }
}
