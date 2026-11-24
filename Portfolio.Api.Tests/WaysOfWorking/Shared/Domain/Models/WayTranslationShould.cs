using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayTranslationShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenLanguageCode = "es";
        const string givenLabel = "Claro";
        const string givenCaption = "Sin jerga";
        const string givenTitle = "Te lo explico sin jerga";
        const string givenDescription = "Antes de escribir una línea hablamos de lo que necesitas.";
        var givenPoints = new List<string> { "Una charla corta", "Un resumen escrito" };

        WayTranslation.Create(
                givenLanguageCode,
                givenLabel,
                givenCaption,
                givenTitle,
                givenDescription,
                givenPoints
            )
            .Match(
                translation =>
                {
                    translation.GetLanguageCode().ShouldBe(givenLanguageCode);
                    translation.GetLabel().ShouldBe(givenLabel);
                    translation.GetCaption().ShouldBe(givenCaption);
                    translation.GetTitle().ShouldBe(givenTitle);
                    translation.GetDescription().ShouldBe(givenDescription);
                    translation.GetPoints().ShouldBe(givenPoints);
                },
                error => error.ShouldBeNull()
            );
    }
}
