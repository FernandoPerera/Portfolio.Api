using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.AddTranslation.Application.UseCases;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Adapters;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Controllers;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Dtos;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.AddTranslation.Infrastructure.Controllers;

public class TimelineTranslationAdderControllerShould
{
    private const string NewLanguage = "en";
    private const string ExistingLanguage = "es";

    private readonly AppDbContext _appDbContext;
    private readonly TimelineTranslationAdderController _controller;

    public TimelineTranslationAdderControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var adder = new TimelineTranslationAdder(
            new EntityFrameworkTimelineEntryFinderAdapter(_appDbContext),
            new EntityFrameworkLanguageExistenceAdapter(_appDbContext),
            new EntityFrameworkTimelineTranslationAdderAdapter(_appDbContext)
        );
        _controller = new TimelineTranslationAdderController(adder, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task AddTranslation()
    {
        var entryId = await AddEntry();
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(entryId, NewLanguage, BuildRequest());

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenTheTranslationIsInvalid()
    {
        var entryId = await AddEntry();
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(entryId, NewLanguage, BuildRequest(title: " "));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(Guid.NewGuid(), NewLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheLanguageDoesNotExist()
    {
        var entryId = await AddEntry();

        var response = await _controller.Add(entryId, NewLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheTranslationAlreadyExists()
    {
        var entryId = await AddEntry();
        await AddLanguage(ExistingLanguage);

        var response = await _controller.Add(entryId, ExistingLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status409Conflict);
    }

    private async Task<Guid> AddEntry()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(
            entryId,
            translations: [TimelineEntryTranslationEntityBuilder.Build(entryId, ExistingLanguage)]
        ));
        await _appDbContext.SaveChangesAsync();

        return entryId;
    }

    private async Task AddLanguage(string languageCode)
    {
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "Language" });
        await _appDbContext.SaveChangesAsync();
    }

    private static AddTimelineTranslationRequest BuildRequest(string title = "Frontend Lead") =>
        new(title, "Technical leadership.");

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
