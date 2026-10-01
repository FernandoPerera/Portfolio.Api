using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Create.Application.UseCases;
using Portfolio.Api.Languages.Create.Infrastructure.Adapters;
using Portfolio.Api.Languages.Create.Infrastructure.Controllers;
using Portfolio.Api.Languages.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Create.Infrastructure.Controllers;

public class LanguageCreatorControllerShould
{
    private readonly LanguageCreatorController _controller;

    public LanguageCreatorControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var creatorRepository = new EntityFrameworkLanguageCreatorAdapter(new AppDbContext(options));
        var creator = new LanguageCreator(creatorRepository);
        _controller = new LanguageCreatorController(creator, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task CreateLanguage()
    {
        var request = new CreateLanguageRequest("es", "Español");

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenLanguageCodeIsInvalid()
    {
        var request = new CreateLanguageRequest("invalid_code", "Español");

        var response = await _controller.Create(request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
}
