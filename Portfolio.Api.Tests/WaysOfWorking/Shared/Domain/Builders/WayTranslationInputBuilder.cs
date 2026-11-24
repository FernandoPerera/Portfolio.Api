using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;

public abstract class WayTranslationInputBuilder
{
    public static WayTranslationInput Build(
        string languageCode = "es",
        string label = "Claro",
        string caption = "Sin jerga",
        string title = "Te lo explico sin jerga",
        string description = "Antes de escribir una línea hablamos de lo que necesitas con palabras normales.",
        List<string>? points = null
    )
    {
        return new WayTranslationInput(
            languageCode,
            label,
            caption,
            title,
            description,
            points ?? ["Una charla corta para entender tu idea"]
        );
    }
}
