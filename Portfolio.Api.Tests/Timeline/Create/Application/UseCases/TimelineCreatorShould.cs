using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Application.Builders;
using Portfolio.Api.Timeline.Create.Application.UseCases;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Portfolio.Api.Timeline.Create.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Application.UseCases;

public class TimelineCreatorShould
{
    private readonly ITimelineCreatorRepository _creatorRepository = Substitute.For<ITimelineCreatorRepository>();
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly TimelineCreator _creator;

    public TimelineCreatorShould()
    {
        _creator = new TimelineCreator(_creatorRepository, _languageExistenceRepository);
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _creatorRepository.CreateEntry(Arg.Any<TimelineEntryToBeCreated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task CreateTimelineEntry()
    {
        var creatorInfo = TimelineCreatorInfoBuilder.Build(
            translations:
            [
                TimelineTranslationInfoBuilder.Build("es"),
                TimelineTranslationInfoBuilder.Build("en")
            ]
        );

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
        await _languageExistenceRepository.Received(2).Exists(Arg.Any<LanguageCode>());
        await _creatorRepository.Received(1).CreateEntry(Arg.Any<TimelineEntryToBeCreated>());
    }
}
