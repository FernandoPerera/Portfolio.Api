using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Timeline.Create.Application.UseCases;
using Portfolio.Api.Timeline.Create.Infrastructure.Adapters;
using Portfolio.Api.Timeline.Create.Infrastructure.Controllers;
using Portfolio.Api.Timeline.Create.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Infrastructure.Controllers;

public class TimelineCreatorControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly TimelineCreatorController _controller;

    public TimelineCreatorControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var creatorRepository = new EntityFrameworkTimelineCreatorAdapter(_appDbContext, new GuidGeneratorIdentifier());
        var languageExistenceRepository = new EntityFrameworkLanguageExistenceAdapter(_appDbContext);
        var creator = new TimelineCreator(creatorRepository, languageExistenceRepository);
        _controller = new TimelineCreatorController(creator, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task CreateTimelineEntry()
    {
        const string languageCode = "es";
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "Spanish" });
        await _appDbContext.SaveChangesAsync();
        var request = BuildRequest(languageCode: languageCode);

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenTheEntryIsInvalid()
    {
        var request = BuildRequest(category: "hobby");

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

    private static CreateTimelineRequest BuildRequest(string category = "work", string languageCode = "es")
    {
        return new CreateTimelineRequest(
            "Lean Mind",
            category,
            "2023-01",
            null,
            ["React", "TypeScript"],
            [new TimelineTranslationRequest(languageCode, "Frontend Lead", "Liderazgo técnico.")]
        );
    }
}
