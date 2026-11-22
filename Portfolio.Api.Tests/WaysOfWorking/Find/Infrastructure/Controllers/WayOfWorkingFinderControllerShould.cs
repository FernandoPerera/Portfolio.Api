using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Infrastructure.Controllers;

public class WayOfWorkingFinderControllerShould
{
    private readonly IWayOfWorkingFinderRepository _repository;
    private readonly WayOfWorkingFinderController _controller;

    public WayOfWorkingFinderControllerShould()
    {
        _repository = Substitute.For<IWayOfWorkingFinderRepository>();
        var finder = new WayOfWorkingFinder(_repository);
        _controller = new WayOfWorkingFinderController(finder, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task FindTheWaysOfWorking()
    {
        var first = WayOfWorkingFoundBuilder.Build(
            position: 1,
            translations: [WayTranslationBuilder.Build(), WayTranslationBuilder.Build("en", "Clear")]
        );
        var second = WayOfWorkingFoundBuilder.Build(position: 2, illustration: "tested");
        _repository.FindWays().Returns(new List<WayOfWorkingFound> { first, second });

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<WayOfWorkingResponse> { ExpectedResponse(first), ExpectedResponse(second) });
    }

    [Fact]
    public async Task FindNoWaysOfWorkingWhenThereAreNone()
    {
        _repository.FindWays().Returns(new List<WayOfWorkingFound>());

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<WayOfWorkingResponse>());
    }

    [Fact]
    public async Task NotFindTheWaysOfWorkingWhenUnexpectedErrorOccurs()
    {
        _repository.FindWays().Returns(new ServiceUnavailableError("Database down"));

        var actionResult = await _controller.Find();

        var problemResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        problemResult.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
    }

    private static WayOfWorkingResponse ExpectedResponse(WayOfWorkingFound way) => new(
        way.GetId(),
        way.GetPosition(),
        way.GetIllustration(),
        way.GetTranslations()
            .Select(translation => new WayTranslationResponse(
                translation.GetLanguageCode(),
                translation.GetLabel(),
                translation.GetCaption(),
                translation.GetTitle(),
                translation.GetDescription(),
                translation.GetPoints()
            ))
            .ToList()
    );
}
