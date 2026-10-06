using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Domain.Models;

public class TimelineEntryToBeUpdatedShould
{
    [Fact]
    public void BeUpdatedWithTheGivenValues()
    {
        var id = Guid.NewGuid();
        var current = TimelineEntryFoundBuilder.Build(id);
        var givenTechnologies = new List<string> { "React", "TypeScript" };
        var english = TimelineTranslationBuilder.Build("en", "Vocational training", "Web application development.");

        TimelineEntryToBeUpdated.Create(
                current,
                "Lean Mind",
                "education",
                "2021-09",
                "2023-06",
                givenTechnologies,
                Option<List<TimelineTranslation>>.Some([english])
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
                    entry.GetTranslations().ShouldBe([english]);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void KeepTheCurrentValuesThatWereNotGiven()
    {
        var current = TimelineEntryFoundBuilder.Build(
            endDate: new DateOnly(2023, 6, 1),
            technologies: ["React", "TypeScript"]
        );

        TimelineEntryToBeUpdated.Create(current, "Lean Mind Labs", null, null, null, null, Option<List<TimelineTranslation>>.None)
            .Match(
                entry =>
                {
                    entry.GetId().ShouldBe(current.GetId());
                    entry.GetOrganization().ShouldBe("Lean Mind Labs");
                    entry.GetCategory().ShouldBe(current.GetCategory());
                    entry.GetStartDate().ShouldBe(new DateOnly(2023, 1, 1));
                    entry.GetEndDate().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1)));
                    entry.GetTechnologies().ShouldBe(current.GetTechnologies());
                    entry.GetTranslations().ShouldBe(current.GetTranslations());
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectUpdateWhenNewStartDateIsAfterTheCurrentEndDate()
    {
        var current = TimelineEntryFoundBuilder.Build(endDate: new DateOnly(2023, 6, 1));

        TimelineEntryToBeUpdated.Create(current, null, null, "2024-01", null, null, Option<List<TimelineTranslation>>.None)
            .Match(
                entry => entry.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }

    [Fact]
    public void RejectUpdateWhenNewEndDateIsBeforeTheCurrentStartDate()
    {
        var current = TimelineEntryFoundBuilder.Build(startDate: new DateOnly(2023, 1, 1));

        TimelineEntryToBeUpdated.Create(current, null, null, null, "2022-12", null, Option<List<TimelineTranslation>>.None)
            .Match(
                entry => entry.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
            );
    }
}
