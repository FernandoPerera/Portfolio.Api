using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Find.Domain.Models;

public class TimelineEntryFoundShould
{
    [Fact]
    public void ReturnTheTranslationOfTheGivenLanguage()
    {
        var spanish = TimelineTranslationBuilder.Build("es");
        var english = TimelineTranslationBuilder.Build("en", "Frontend Lead", "Technical leadership.");
        var entry = TimelineEntryFoundBuilder.Build(translations: [spanish, english]);
        var languageCode = LanguageCode.Create("en").IfLeft(error => throw new Exception(error.Message));

        entry.GetTranslation(languageCode)
            .Match(
                translation => translation.ShouldBe(english),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void FailWhenTheLanguageIsNotInTheEntry()
    {
        var entry = TimelineEntryFoundBuilder.Build(translations: [TimelineTranslationBuilder.Build("es")]);
        var languageCode = LanguageCode.Create("fr").IfLeft(error => throw new Exception(error.Message));

        entry.GetTranslation(languageCode)
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
                    .Message.ShouldBe("Timeline translation for language 'fr' was not found")
            );
    }
}
