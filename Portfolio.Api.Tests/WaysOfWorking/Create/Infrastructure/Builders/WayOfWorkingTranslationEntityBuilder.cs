using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;

public static class WayOfWorkingTranslationEntityBuilder
{
    public static WayOfWorkingTranslationEntity Build(
        Guid wayId,
        string languageCode = "es",
        string label = "Claro",
        string caption = "Sin jerga",
        string title = "Te lo explico sin jerga",
        string description = "Antes de escribir una línea hablamos de lo que necesitas con palabras normales.",
        List<string>? points = null
    )
    {
        return new WayOfWorkingTranslationEntity
        {
            WayId = wayId,
            LanguageCode = languageCode,
            Label = label,
            Caption = caption,
            Title = title,
            Description = description,
            Points = points ?? ["Una charla corta para entender tu idea"]
        };
    }
}
