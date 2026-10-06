using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Create.Infrastructure.Dtos;
using Portfolio.Api.Timeline.Update.Application.UseCases;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;
using Portfolio.Api.Timeline.Update.Infrastructure.Controllers;
using Portfolio.Api.Timeline.Update.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Infrastructure.Controllers;

public class TimelineUpdaterControllerShould
{
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
            new EntityFrameworkTimelineUpdaterAdapter(_appDbContext),
            new EntityFrameworkLanguageExistenceAdapter(_appDbContext)
        );
        _controller = new TimelineUpdaterController(updater, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task UpdateTimelineEntry()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(entryId, BuildRequest(organization: "Lean Mind Labs"));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateTimelineEntryTranslations()
    {
        const string languageCode = "en";
        var entryId = Guid.NewGuid();
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "English" });
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();
        var request = BuildRequest(
            translations: [new TimelineTranslationRequest(languageCode, "Frontend Lead", "Technical leadership.")]
        );

        var response = await _controller.Update(entryId, request);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenTheUpdateIsInvalid()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(entryId, BuildRequest(category: "hobby"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        var response = await _controller.Update(Guid.NewGuid(), BuildRequest(organization: "Lean Mind Labs"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();
        var request = BuildRequest(
            translations: [new TimelineTranslationRequest("fr", "Frontend Lead", "Leadership technique.")]
        );

        var response = await _controller.Update(entryId, request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    private static UpdateTimelineRequest BuildRequest(
        string? organization = null,
        string? category = null,
        List<TimelineTranslationRequest>? translations = null
    )
    {
        return new UpdateTimelineRequest(organization, category, null, null, null, translations);
    }
}
