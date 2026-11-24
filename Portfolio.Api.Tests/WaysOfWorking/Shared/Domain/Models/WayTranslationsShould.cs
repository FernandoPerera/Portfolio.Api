using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Inputs;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Models;

public class WayTranslationsShould
{
    [Fact]
    public void BeCreated()
    {
        var spanish = WayTranslationInputBuilder.Build("es");
        var english = WayTranslationInputBuilder.Build("en");

        WayTranslations.Create([spanish, english])
            .Match(
                translations => translations.GetValues()
                    .Select(translation => translation.GetLanguageCode())
                    .ShouldBe(["es", "en"]),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectTranslationsWhenThereAreNone()
    {
        WayTranslations.Create([])
            .Match(
                translations => translations.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("At least one translation is required")
            );
    }

    [Fact]
    public void RejectTranslationsWhenALanguageIsRepeated()
    {
        var first = WayTranslationInputBuilder.Build("es", "Claro");
        var second = WayTranslationInputBuilder.Build("es", "Probado");

        WayTranslations.Create([first, second])
            .Match(
                translations => translations.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Translations cannot repeat the same language")
            );
    }
}
