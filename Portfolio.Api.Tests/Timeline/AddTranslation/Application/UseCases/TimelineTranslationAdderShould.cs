using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.AddTranslation.Application.Dtos;
using Portfolio.Api.Timeline.AddTranslation.Application.UseCases;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Portfolio.Api.Timeline.AddTranslation.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.AddTranslation.Application.UseCases;

public class TimelineTranslationAdderShould
{
    private readonly Guid _entryId = Guid.NewGuid();
    private readonly ITimelineEntryFinderRepository _finderRepository = Substitute.For<ITimelineEntryFinderRepository>();
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly ITimelineTranslationAdderRepository _adderRepository = Substitute.For<ITimelineTranslationAdderRepository>();
    private readonly TimelineTranslationAdder _adder;

    public TimelineTranslationAdderShould()
    {
        _adder = new TimelineTranslationAdder(_finderRepository, _languageExistenceRepository, _adderRepository);
        _finderRepository.FindEntry(Arg.Is<TimelineId>(id => id.ToGuid() == _entryId)).Returns(
            TimelineEntryFoundBuilder.Build(_entryId, translations: [TimelineTranslationBuilder.Build("es")])
        );
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _adderRepository.AddTranslation(Arg.Any<TimelineTranslationToBeAdded>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task AddTranslation()
    {
        const string givenLanguageCode = "en";
        const string givenTitle = "Frontend Lead";
        var info = new TimelineTranslationAdderInfo(_entryId, givenLanguageCode, givenTitle, "Technical leadership.");

        var result = await _adder.Execute(info);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _adderRepository.Received(1).AddTranslation(Arg.Is<TimelineTranslationToBeAdded>(translation =>
                    translation.GetEntryId() == _entryId
                    && translation.GetLanguageCode() == givenLanguageCode
                    && translation.GetTitle() == givenTitle
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
