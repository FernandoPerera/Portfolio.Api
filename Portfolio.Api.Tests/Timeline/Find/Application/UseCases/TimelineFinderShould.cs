using NSubstitute;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Timeline.Find.Application.UseCases;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Find.Application.UseCases;

public class TimelineFinderShould
{
    private readonly ITimelineFinderRepository _repository;
    private readonly TimelineFinder _finder;

    public TimelineFinderShould()
    {
        _repository = Substitute.For<ITimelineFinderRepository>();
        _finder = new TimelineFinder(_repository);
    }

    [Fact]
    public async Task FindTimelineEntries()
    {
        var entries = new List<TimelineEntryFound>
        {
            TimelineEntryFoundBuilder.Build(organization: "Lean Mind"),
            TimelineEntryFoundBuilder.Build(organization: "Universidad de La Laguna", category: "education")
        };
        _repository.FindEntries().Returns(entries);

        var result = await _finder.Execute();

        result.Match(
            found => found.ShouldBe(entries),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindNoEntriesWhenTimelineIsEmpty()
    {
        _repository.FindEntries().Returns(new List<TimelineEntryFound>());

        var result = await _finder.Execute();

        result.Match(
            found => found.ShouldBeEmpty(),
            error => error.ShouldBeNull()
        );
    }
}
