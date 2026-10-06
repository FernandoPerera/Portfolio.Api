using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Timeline.Delete.Application.Dtos;
using Portfolio.Api.Timeline.Delete.Application.UseCases;
using Portfolio.Api.Timeline.Delete.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Delete.Application.UseCases;

public class TimelineDeleterShould
{
    private readonly ITimelineDeleterRepository _deleterRepository = Substitute.For<ITimelineDeleterRepository>();
    private readonly TimelineDeleter _deleter;

    public TimelineDeleterShould()
    {
        _deleter = new TimelineDeleter(_deleterRepository);
    }

    [Fact]
    public async Task DeleteTimelineEntry()
    {
        var entryId = Guid.NewGuid();
        _deleterRepository.DeleteEntry(Arg.Any<TimelineId>()).Returns(Unit.Default);

        var result = await _deleter.Execute(new TimelineDeleterInfo(entryId));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _deleterRepository.Received(1).DeleteEntry(Arg.Is<TimelineId>(id => id.ToGuid() == entryId));
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenIdIsInvalid()
    {
        var result = await _deleter.Execute(new TimelineDeleterInfo(Guid.Empty));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ValidationException>();
                _deleterRepository.DidNotReceive().DeleteEntry(Arg.Any<TimelineId>());
            }
        );
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        _deleterRepository.DeleteEntry(Arg.Any<TimelineId>())
            .Returns(Error.New(new NotFoundException("Timeline entry was not found")));

        var result = await _deleter.Execute(new TimelineDeleterInfo(Guid.NewGuid()));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
        );
    }
}
