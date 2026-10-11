using LanguageExt;
using NSubstitute;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Update.Application.Builders;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Application.UseCases;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Application.UseCases;

public class TimelineUpdaterShould
{
    private readonly Guid _entryId = Guid.NewGuid();
    private readonly ITimelineEntryFinderRepository _finderRepository = Substitute.For<ITimelineEntryFinderRepository>();
    private readonly ITimelineUpdaterRepository _updaterRepository = Substitute.For<ITimelineUpdaterRepository>();
    private readonly TimelineUpdater _updater;

    public TimelineUpdaterShould()
    {
        _updater = new TimelineUpdater(_finderRepository, _updaterRepository);
        _finderRepository.FindEntry(Arg.Is<TimelineId>(id => id.ToGuid() == _entryId)).Returns(TimelineEntryFoundBuilder.Build(_entryId));
        _updaterRepository.UpdateEntry(Arg.Any<TimelineEntryToBeUpdated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task UpdateTimelineEntry()
    {
        const string givenOrganization = "Lean Mind";
        const string givenTitle = "Tech lead";
        var updaterInfo = TimelineUpdaterInfoBuilder.Build(_entryId, organization: givenOrganization, title: givenTitle);

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _updaterRepository.Received(1).UpdateEntry(Arg.Is<TimelineEntryToBeUpdated>(entry =>
                    entry.GetId() == _entryId
                    && entry.GetOrganization() == givenOrganization
                    && entry.GetTitle() == givenTitle
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
