using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;

public abstract class WayTranslationBuilder
{
    public static WayTranslation Build(
        string languageCode = "es",
        string label = "Claro",
        string caption = "Sin jerga",
        string title = "Te lo explico sin jerga",
        string description = "Antes de escribir una línea hablamos de lo que necesitas con palabras normales.",
        List<string>? points = null
    )
    {
        return WayTranslation.Create(
                languageCode,
                label,
                caption,
                title,
                description,
                points ?? ["Una charla corta para entender tu idea"]
            )
            .IfLeft(error => throw new Exception("Error in WayTranslationBuilder, error: " + error.Message));
    }
}
