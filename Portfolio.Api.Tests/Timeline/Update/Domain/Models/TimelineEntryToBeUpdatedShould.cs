using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdatedShould
{
    [Fact]
    public void BeUpdatedWithTheGivenValues()
    {
        const string givenLanguageCode = "en";
        const string givenTitle = "Vocational training";
        const string givenDescription = "Web application development.";
        var id = Guid.NewGuid();
        var current = TimelineEntryFoundBuilder.Build(
            id,
            translations: [TimelineTranslationBuilder.Build(givenLanguageCode)]
        );
        var givenTechnologies = new List<string> { "React", "TypeScript" };

        TimelineEntryToBeUpdated.Create(
                current,
                "Lean Mind",
                "education",
                "2021-09",
                "2023-06",
                givenTechnologies,
                givenTitle,
                givenDescription,
                givenLanguageCode
            )
            .Match(
                entry =>
                {
                    entry.GetId().ShouldBe(id);
                    entry.GetOrganization().ShouldBe("Lean Mind");
                    entry.GetCategory().ShouldBe("education");
                    entry.GetStartDate().ShouldBe(new DateOnly(2021, 9, 1));
                    entry.GetEndDate().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1)));
                    entry.GetTechnologies().ShouldBe(givenTechnologies);
                    entry.GetLanguageCode().ShouldBe(givenLanguageCode);
                    entry.GetTitle().ShouldBe(givenTitle);
                    entry.GetDescription().ShouldBe(givenDescription);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void KeepTheCurrentValuesThatWereNotGiven()
    {
        const string givenOrganization = "Lean Mind Labs";
        var translation = TimelineTranslationBuilder.Build("es");
        var current = TimelineEntryFoundBuilder.Build(
            endDate: new DateOnly(2023, 6, 1),
            technologies: ["React", "TypeScript"],
            translations: [translation]
        );

        TimelineEntryToBeUpdated.Create(current, givenOrganization, null, null, null, null, null, null, translation.GetLanguageCode())
            .Match(
                entry =>
                {
                    entry.GetId().ShouldBe(current.GetId());
                    entry.GetOrganization().ShouldBe(givenOrganization);
                    entry.GetCategory().ShouldBe(current.GetCategory());
                    entry.GetStartDate().ShouldBe(new DateOnly(2023, 1, 1));
                    entry.GetEndDate().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1)));
                    entry.GetTechnologies().ShouldBe(current.GetTechnologies());
                    entry.GetTitle().ShouldBe(translation.GetTitle());
                    entry.GetDescription().ShouldBe(translation.GetDescription());
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectUpdateWhenTheLanguageIsNotInTheEntry()
    {
        var current = TimelineEntryFoundBuilder.Build(translations: [TimelineTranslationBuilder.Build("es")]);

        TimelineEntryToBeUpdated.Create(current, null, null, null, null, null, null, null, "fr")
            .Match(
                entry => entry.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
            );
    }

    [Fact]
    public void RejectUpdateWhenNewStartDateIsAfterTheCurrentEndDate()
    {
        var current = TimelineEntryFoundBuilder.Build(endDate: new DateOnly(2023, 6, 1));

        TimelineEntryToBeUpdated.Create(current, null, null, "2024-01", null, null, null, null, "es")
            .Match(
                entry => entry.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }

    [Fact]
    public void RejectUpdateWhenNewEndDateIsBeforeTheCurrentStartDate()
    {
        var current = TimelineEntryFoundBuilder.Build(startDate: new DateOnly(2023, 1, 1));

        TimelineEntryToBeUpdated.Create(current, null, null, null, "2022-12", null, null, null, "es")
            .Match(
                entry => entry.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }
}
