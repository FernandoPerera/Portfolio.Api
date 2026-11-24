using Portfolio.Api.WaysOfWorking.Create.Application.Dtos;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Application.Builders;

public abstract class WayTranslationInfoBuilder
{
    public static WayTranslationInfo Build(
        string languageCode = "es",
        string label = "Claro",
        string caption = "Sin jerga",
        string title = "Te lo explico sin jerga",
        string description = "Antes de escribir una línea hablamos de lo que necesitas con palabras normales.",
        List<string>? points = null
    )
    {
        return new WayTranslationInfo(
            languageCode,
            label,
            caption,
            title,
            description,
            points ?? ["Una charla corta para entender tu idea"]
        );
    }
}
