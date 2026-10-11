using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.AddTranslation.Domain.Models;

public class TimelineTranslationToBeAddedShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenTitle = "Frontend Lead";
        const string givenDescription = "Technical leadership.";
        var current = TimelineEntryFoundBuilder.Build(translations: [TimelineTranslationBuilder.Build("es")]);

        TimelineTranslationToBeAdded.Create(current, "EN", givenTitle, givenDescription)
            .Match(
                translation =>
                {
                    translation.GetEntryId().ShouldBe(current.GetId());
                    translation.GetLanguageCode().ShouldBe("en");
                    translation.GetTitle().ShouldBe(givenTitle);
                    translation.GetDescription().ShouldBe(givenDescription);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectTranslationWhenTheEntryAlreadyHasTheLanguage()
    {
        var current = TimelineEntryFoundBuilder.Build(translations: [TimelineTranslationBuilder.Build("es")]);

        TimelineTranslationToBeAdded.Create(current, "es", "Frontend Lead", "Technical leadership.")
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ConflictException>()
                    .Message.ShouldBe("Timeline entry already has a translation for language 'es'")
            );
    }

    [Fact]
    public void RejectTranslationWhenLanguageCodeIsInvalid()
    {
        TimelineTranslationToBeAdded.Create(TimelineEntryFoundBuilder.Build(), "eng", "Frontend Lead", "Technical leadership.")
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }

    [Fact]
    public void RejectTranslationWhenTitleIsEmpty()
    {
        var current = TimelineEntryFoundBuilder.Build(translations: [TimelineTranslationBuilder.Build("es")]);

        TimelineTranslationToBeAdded.Create(current, "en", " ", "Technical leadership.")
            .Match(
                translation => translation.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }
}
