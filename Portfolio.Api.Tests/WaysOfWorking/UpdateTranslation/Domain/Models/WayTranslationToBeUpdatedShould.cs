using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Domain.Models;

public class WayTranslationToBeUpdatedShould
{
    private readonly Guid _wayId = Guid.NewGuid();

    [Fact]
    public void UpdateOnlyTheFieldsGiven()
    {
        var current = WayOfWorkingFoundBuilder.Build(
            _wayId,
            translations:
            [
                WayTranslationBuilder.Build("es", "Claro", "Sin jerga", "Te lo explico", "Descripción", ["Una charla"]),
                WayTranslationBuilder.Build("en", "Clear")
            ]
        );

        WayTranslationToBeUpdated.Create(current, "ES", "Claro y simple", null, null, "Nueva descripción", ["Una charla", "Un resumen"])
            .Match(
                translation =>
                {
                    translation.GetWayId().ShouldBe(_wayId);
                    translation.GetLanguageCode().ShouldBe("es");
                    translation.GetLabel().ShouldBe("Claro y simple");
                    translation.GetCaption().ShouldBe("Sin jerga");
                    translation.GetTitle().ShouldBe("Te lo explico");
                    translation.GetDescription().ShouldBe("Nueva descripción");
                    translation.GetPoints().ShouldBe(["Una charla", "Un resumen"]);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void KeepTheCurrentTranslationWhenNothingIsGiven()
    {
        var current = WayOfWorkingFoundBuilder.Build(
            _wayId,
            translations: [WayTranslationBuilder.Build("es", "Claro", "Sin jerga", "Te lo explico", "Descripción", ["Una charla"])]
        );

        WayTranslationToBeUpdated.Create(current, "es", null, null, null, null, null)
            .Match(
                translation =>
                {
                    translation.GetLabel().ShouldBe("Claro");
                    translation.GetCaption().ShouldBe("Sin jerga");
                    translation.GetTitle().ShouldBe("Te lo explico");
                    translation.GetDescription().ShouldBe("Descripción");
                    translation.GetPoints().ShouldBe(["Una charla"]);
                },
                error => error.ShouldBeNull()
            );
    }
}
