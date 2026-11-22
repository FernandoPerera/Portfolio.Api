using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Domain.Builders;

public abstract class WayTranslationToBeAddedBuilder
{
    public static WayTranslationToBeAdded Build(
        Guid? wayId = null,
        string languageCode = "en",
        string label = "Clear",
        string caption = "No jargon",
        string title = "I explain it without jargon",
        string description = "Before writing a line we talk in plain words.",
        List<string>? points = null
    )
    {
        return WayTranslationToBeAdded.Create(
                wayId ?? Guid.NewGuid(),
                WayTranslationInputBuilder.Build(languageCode, label, caption, title, description, points)
            )
            .IfLeft(error => throw new Exception("Error in WayTranslationToBeAddedBuilder, error: " + error.Message));
    }
}
