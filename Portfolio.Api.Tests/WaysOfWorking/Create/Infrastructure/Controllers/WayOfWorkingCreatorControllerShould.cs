using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Create.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Controllers;

public class WayOfWorkingCreatorControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly WayOfWorkingCreatorController _controller;

    public WayOfWorkingCreatorControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        var creatorRepository = new EntityFrameworkWayOfWorkingCreatorAdapter(_appDbContext, new GuidGeneratorIdentifier());
        var languageExistenceRepository = new EntityFrameworkLanguageExistenceAdapter(_appDbContext);
        var creator = new WayOfWorkingCreator(creatorRepository, languageExistenceRepository);
        _controller = new WayOfWorkingCreatorController(creator, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task CreateWayOfWorking()
    {
        const string languageCode = "es";
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "Spanish" });
        await _appDbContext.SaveChangesAsync();
        var request = BuildRequest(languageCode: languageCode);

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingIsInvalid()
    {
        var request = BuildRequest(illustration: "Plain Language");

        var response = await _controller.Create(request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var request = BuildRequest(languageCode: "fr");

        var response = await _controller.Create(request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    private static CreateWayOfWorkingRequest BuildRequest(
        string illustration = "plain-language",
        string languageCode = "es"
    )
    {
        return new CreateWayOfWorkingRequest(
            illustration,
            [
                new WayTranslationRequest(
                    languageCode,
                    "Claro",
                    "Sin jerga",
                    "Te lo explico sin jerga",
                    "Antes de escribir una línea hablamos de lo que necesitas.",
                    ["Una charla corta para entender tu idea"]
                )
            ]
        );
    }
}
