using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Profiles.Find.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Infrastructure.Controllers;

public class ProfileFinderControllerShould
{
    private readonly IProfileFinderRepository _repository;
    private readonly ProfileFinderController _controller;

    public ProfileFinderControllerShould()
    {
        _repository = Substitute.For<IProfileFinderRepository>();
        var finder = new ProfileFinder(_repository);
        _controller = new ProfileFinderController(finder, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task FindTheProfile()
    {
        var spanish = ProfileTranslationFoundBuilder.Build(
            "es", "Mucho gusto :)", "Arquitecto de software cervecero"
        );
        var english = ProfileTranslationFoundBuilder.Build(
            "en", "Nice to meet you :)", "Beer software architect"
        );
        var domainProfile = ProfileFoundBuilder.Build(
            name: "Fernando",
            enterprise: "Lean Mind",
            translations: [spanish, english]
        );
        _repository.FindProfile().Returns(domainProfile);

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(
            new ProfileResponse(
                domainProfile.GetName(),
                domainProfile.GetEnterprise(),
                [
                    new ProfileTranslationResponse(spanish.GetLanguageCode(), spanish.GetPresentation(), spanish.GetRole()),
                    new ProfileTranslationResponse(english.GetLanguageCode(), english.GetPresentation(), english.GetRole())
                ]
            )
        );
    }

    [Fact]
    public async Task NotFindTheProfileWhenNotExists()
    {
        var notFoundError = Error.New(new NotFoundException("Profile was not found"));
        _repository.FindProfile().Returns(notFoundError);

        var actionResult = await _controller.Find();

        var objectResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task NotFindTheProfileWhenUnexpectedErrorOccurs()
    {
        var serviceUnavailableError = Error.New(new ServiceUnavailableException("Database down"));
        _repository.FindProfile().Returns(serviceUnavailableError);

        var actionResult = await _controller.Find();

        var problemResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        problemResult.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
    }
}
