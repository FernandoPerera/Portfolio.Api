using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Find.Domain.Models;

public class TimelineEntryFoundShould
{
    private const string Organization = "Lean Mind";
    private const string Category = "work";
    private static readonly DateOnly StartDate = new(2023, 1, 1);
    private static readonly DateOnly EndDate = new(2024, 6, 1);
    private static readonly List<string> Technologies = ["C#", ".NET"];

    [Fact]
    public void BeCreatedWithAnEndDate()
    {
        var translation = TimelineTranslationBuilder.Build();

        TimelineEntryFound.Create(Organization, Category, StartDate, EndDate, Technologies, [translation])
            .Match(
                entry =>
                {
                    entry.GetOrganization().ShouldBe(Organization);
                    entry.GetCategory().ShouldBe(Category);
                    entry.GetStartDate().ShouldBe(StartDate);
                    entry.GetEndDate().ShouldBe(EndDate);
                    entry.GetTechnologies().ShouldBe(Technologies);
                    entry.GetTranslations().ShouldBe([translation]);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeCreatedWithoutAnEndDateWhenItIsOngoing()
    {
        TimelineEntryFound.Create(Organization, Category, StartDate, null, Technologies, [])
            .Match(
                entry => entry.GetEndDate().IsNone.ShouldBeTrue(),
                error => error.ShouldBeNull()
            );
    }
}
