using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Delete.Application.UseCases;
using Portfolio.Api.Timeline.Delete.Infrastructure.Adapters;
using Portfolio.Api.Timeline.Delete.Infrastructure.Controllers;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Delete.Infrastructure.Controllers;

public class TimelineDeleterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly TimelineDeleterController _controller;

    public TimelineDeleterControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var deleter = new TimelineDeleter(new EntityFrameworkTimelineDeleterAdapter(_appDbContext));
        _controller = new TimelineDeleterController(deleter, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task DeleteTimelineEntry()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Delete(entryId);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenIdIsInvalid()
    {
        var response = await _controller.Delete(Guid.Empty);

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        var response = await _controller.Delete(Guid.NewGuid());

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
