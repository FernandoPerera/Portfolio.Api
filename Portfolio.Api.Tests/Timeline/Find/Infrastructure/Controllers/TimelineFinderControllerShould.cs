using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Find.Application.UseCases;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Ports;
using Portfolio.Api.Timeline.Find.Infrastructure.Controllers;
using Portfolio.Api.Timeline.Find.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Find.Infrastructure.Controllers;

public class TimelineFinderControllerShould
{
    private readonly ITimelineFinderRepository _repository;
    private readonly TimelineFinderController _controller;

    public TimelineFinderControllerShould()
    {
        _repository = Substitute.For<ITimelineFinderRepository>();
        var finder = new TimelineFinder(_repository);
        _controller = new TimelineFinderController(finder, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task FindTheTimeline()
    {
        var finished = TimelineEntryFoundBuilder.Build(
            endDate: new DateOnly(2023, 6, 1),
            translations: [TimelineTranslationBuilder.Build(), TimelineTranslationBuilder.Build("en")]
        );
        var ongoing = TimelineEntryFoundBuilder.Build();
        _repository.FindEntries().Returns(new List<TimelineEntryFound> { finished, ongoing });

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<TimelineEntryResponse> { ExpectedResponse(finished), ExpectedResponse(ongoing) });
    }

    [Fact]
    public async Task FindAnEmptyTimeline()
    {
        _repository.FindEntries().Returns(new List<TimelineEntryFound>());

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<TimelineEntryResponse>());
    }

    [Fact]
    public async Task NotFindTheTimelineWhenUnexpectedErrorOccurs()
    {
        var serviceUnavailableError = Error.New(new ServiceUnavailableException("Database down"));
        _repository.FindEntries().Returns(serviceUnavailableError);

        var actionResult = await _controller.Find();

        var problemResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        problemResult.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
    }

    private static TimelineEntryResponse ExpectedResponse(TimelineEntryFound entry) => new(
        entry.GetOrganization(),
        entry.GetCategory(),
        entry.GetStartDate(),
        entry.GetEndDate(),
        entry.GetTechnologies(),
        entry.GetTranslations()
            .Select(translation => new TimelineTranslationResponse(
                translation.GetLanguageCode(),
                translation.GetTitle(),
                translation.GetDescription()
            ))
            .ToList()
    );
}
