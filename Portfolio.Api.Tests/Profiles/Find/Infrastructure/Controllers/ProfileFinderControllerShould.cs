using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Profiles.Find.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Domain.Errors;
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
        _controller = new ProfileFinderController(finder);
    }

    [Fact]
    public async Task FindTheProfile()
    {
        var domainProfile = ProfileFoundBuilder.Build(
            name: "Fernando",
            jobRole: "Beer software architect",
            presentation: "Mucho gusto :)",
            enterprise: "Lean Mind"
        );
        _repository.FindProfile().Returns(domainProfile);

        var actionResult = await _controller.Find(acceptLanguage: "es");

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var profileResponse = okResult.Value.ShouldBeOfType<ProfileResponse>();
        profileResponse.Name.ShouldBe("Fernando");
        profileResponse.JobRole.ShouldBe("Beer software architect");
        profileResponse.Presentation.ShouldBe("Mucho gusto :)");
        profileResponse.Enterprise.ShouldBe("Lean Mind");
    }

    [Fact]
    public async Task NotFindTheProfileWhenNotExists()
    {
        var notFoundError = Error.New(new NotFoundException("Profile was not found"));
        _repository.FindProfile().Returns(notFoundError);

        var actionResult = await _controller.Find(acceptLanguage: "es");

        var notFoundResult = actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        notFoundResult.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task NotFindTheProfileWhenUnexpectedErrorOccurs()
    {
        var serviceUnavailableError = Error.New(new ServiceUnavailableException("Database down"));
        _repository.FindProfile().Returns(serviceUnavailableError);

        var actionResult = await _controller.Find(acceptLanguage: "es");

        var problemResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        problemResult.StatusCode.ShouldBe(500);
    }
}
