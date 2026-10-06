using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Portfolio.Api.Languages.Find.Application.UseCases;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Languages.Find.Domain.Ports;
using Portfolio.Api.Languages.Find.Infrastructure.Controllers;
using Portfolio.Api.Languages.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Languages.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Find.Infrastructure.Controllers;

public class LanguageFinderControllerShould
{
    private readonly ILanguageFinderRepository _repository;
    private readonly LanguageFinderController _controller;

    public LanguageFinderControllerShould()
    {
        _repository = Substitute.For<ILanguageFinderRepository>();
        var finder = new LanguageFinder(_repository);
        _controller = new LanguageFinderController(finder, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task FindTheLanguages()
    {
        _repository.FindLanguages().Returns(new List<LanguageFound>
        {
            LanguageFoundBuilder.Build("en", "English"),
            LanguageFoundBuilder.Build("es", "Español")
        });

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<LanguageResponse>
        {
            new("en", "English"),
            new("es", "Español")
        });
    }

    [Fact]
    public async Task FindNoLanguagesWhenThereAreNone()
    {
        _repository.FindLanguages().Returns(new List<LanguageFound>());

        var actionResult = await _controller.Find();

        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeEquivalentTo(new List<LanguageResponse>());
    }

    [Fact]
    public async Task NotFindTheLanguagesWhenUnexpectedErrorOccurs()
    {
        var serviceUnavailableError = Error.New(new ServiceUnavailableException("Database down"));
        _repository.FindLanguages().Returns(serviceUnavailableError);

        var actionResult = await _controller.Find();

        var problemResult = actionResult.Result.ShouldBeOfType<ObjectResult>();
        problemResult.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
    }
}
