using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Application.Builders;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Update.Application.Builders;
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
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly TimelineUpdater _updater;

    public TimelineUpdaterShould()
    {
        _updater = new TimelineUpdater(_finderRepository, _updaterRepository, _languageExistenceRepository);
        _finderRepository.FindEntry(_entryId).Returns(TimelineEntryFoundBuilder.Build(_entryId));
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _updaterRepository.UpdateEntry(Arg.Any<TimelineEntryToBeUpdated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task UpdateTimelineEntry()
    {
        var updaterInfo = TimelineUpdaterInfoBuilder.Build(
            _entryId,
            organization: "Lean Mind",
            translations:
            [
                TimelineTranslationInfoBuilder.Build("es"),
                TimelineTranslationInfoBuilder.Build("en")
            ]
        );

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _languageExistenceRepository.Received(2).Exists(Arg.Any<LanguageCode>());
                _updaterRepository.Received(1).UpdateEntry(Arg.Is<TimelineEntryToBeUpdated>(entry => entry.GetId() == _entryId));
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task UpdateTimelineEntryWithoutCheckingLanguagesWhenTranslationsAreNotGiven()
    {
        var updaterInfo = TimelineUpdaterInfoBuilder.Build(_entryId, organization: "Lean Mind");

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _languageExistenceRepository.DidNotReceive().Exists(Arg.Any<LanguageCode>());
                _updaterRepository.Received(1).UpdateEntry(Arg.Any<TimelineEntryToBeUpdated>());
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenTheUpdateIsInvalid()
    {
        var updaterInfo = TimelineUpdaterInfoBuilder.Build(_entryId, category: "hobby");

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ValidationException>();
                _updaterRepository.DidNotReceive().UpdateEntry(Arg.Any<TimelineEntryToBeUpdated>());
            }
        );
    }

    [Fact]
    public async Task FailWhenALanguageDoesNotExist()
    {
        const string missingLanguage = "fr";
        var updaterInfo = TimelineUpdaterInfoBuilder.Build(
            _entryId,
            translations:
            [
                TimelineTranslationInfoBuilder.Build("es"),
                TimelineTranslationInfoBuilder.Build(missingLanguage)
            ]
        );
        _languageExistenceRepository.Exists(Arg.Is<LanguageCode>(code => code.ToString() == missingLanguage))
            .Returns(Error.New(new NotFoundException($"Language '{missingLanguage}' was not found")));

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<NotFoundException>();
                _updaterRepository.DidNotReceive().UpdateEntry(Arg.Any<TimelineEntryToBeUpdated>());
            }
        );
    }
}
