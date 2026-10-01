using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Timeline.Create.Domain.Builders;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Models;

public class TimelineTranslationsShould
{
    [Fact]
    public void BeCreated()
    {
        var spanish = TimelineTranslationBuilder.Build("es");
        var english = TimelineTranslationBuilder.Build("en");

        TimelineTranslations.Create([spanish, english])
            .Match(
                translations => translations.GetValues().ShouldBe([spanish, english]),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectTranslationsWhenThereAreNone()
    {
        TimelineTranslations.Create([])
            .Match(
                translations => translations.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("At least one translation is required")
            );
    }

    [Fact]
    public void RejectTranslationsWhenALanguageIsRepeated()
    {
        var first = TimelineTranslationBuilder.Build("es", "Frontend Lead");
        var second = TimelineTranslationBuilder.Build("es", "Líder Frontend");

        TimelineTranslations.Create([first, second])
            .Match(
                translations => translations.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Translations cannot repeat the same language")
            );
    }
}
