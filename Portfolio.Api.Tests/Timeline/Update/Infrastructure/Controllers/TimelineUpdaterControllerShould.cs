using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Update.Application.UseCases;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;
using Portfolio.Api.Timeline.Update.Infrastructure.Controllers;
using Portfolio.Api.Timeline.Update.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Infrastructure.Controllers;

public class TimelineUpdaterControllerShould
{
    private const string DefaultLanguage = "es";

    private readonly AppDbContext _appDbContext;
    private readonly TimelineUpdaterController _controller;

    public TimelineUpdaterControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var updater = new TimelineUpdater(
            new EntityFrameworkTimelineEntryFinderAdapter(_appDbContext),
            new EntityFrameworkTimelineUpdaterAdapter(_appDbContext)
        );
        _controller = new TimelineUpdaterController(updater, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task UpdateTimelineEntry()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(entryId, DefaultLanguage, BuildRequest(organization: "Lean Mind Labs"));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateTimelineEntryTranslation()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();
        var request = BuildRequest(title: "Tech Lead", description: "Technical leadership.");

        var response = await _controller.Update(entryId, DefaultLanguage, request);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenTheUpdateIsInvalid()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(entryId, DefaultLanguage, BuildRequest(category: "hobby"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        var response = await _controller.Update(Guid.NewGuid(), DefaultLanguage, BuildRequest(organization: "Lean Mind Labs"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheTranslationDoesNotExist()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(entryId, "fr", BuildRequest(title: "Tech Lead"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    private static UpdateTimelineRequest BuildRequest(
        string? organization = null,
        string? category = null,
        string? title = null,
        string? description = null
    )
    {
        return new UpdateTimelineRequest(organization, category, null, null, null, title, description);
    }
}
