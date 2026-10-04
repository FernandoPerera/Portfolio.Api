using LanguageExt;
using Portfolio.Api.Tests.Timeline.Create.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Domain.Models;

public class TimelineEntryToBeCreatedShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenOrganization = "Lean Mind";
        const string givenCategory = "education";
        var givenTechnologies = new List<string> { "React", "TypeScript" };
        var spanish = TimelineTranslationBuilder.Build("es", "Ciclo formativo", "Desarrollo de aplicaciones web.");
        var english = TimelineTranslationBuilder.Build("en", "Vocational training", "Web application development.");

        TimelineEntryToBeCreated.Create(
                givenOrganization,
                givenCategory,
                "2021-09",
                "2023-06",
                givenTechnologies,
                [spanish, english]
            )
            .Match(
                entry =>
                {
                    entry.GetOrganization().ShouldBe(givenOrganization);
                    entry.GetCategory().ShouldBe(givenCategory);
                    entry.GetStartDate().ShouldBe(new DateOnly(2021, 9, 1));
                    entry.GetEndDate().ShouldBe(Option<DateOnly>.Some(new DateOnly(2023, 6, 1)));
                    entry.GetTechnologies().ShouldBe(givenTechnologies);
                    entry.GetTranslations().ShouldBe([spanish, english]);
                },
                error => error.ShouldBeNull()
            );
    }
}
