using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Update.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Update.Infrastructure.Controllers;

public class WayOfWorkingUpdaterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly WayOfWorkingUpdaterController _controller;

    public WayOfWorkingUpdaterControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages("es");
        var updater = new WayOfWorkingUpdater(
            new EntityFrameworkWayOfWorkingByIdFinderAdapter(_appDbContext),
            new EntityFrameworkWayOfWorkingUpdaterAdapter(_appDbContext)
        );
        _controller = new WayOfWorkingUpdaterController(updater, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task UpdateTheWayOfWorking()
    {
        var wayId = await AddWay();

        var response = await _controller.Update(wayId, new UpdateWayOfWorkingRequest("fast-response"));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenTheIllustrationIsInvalid()
    {
        var wayId = await AddWay();

        var response = await _controller.Update(wayId, new UpdateWayOfWorkingRequest("Fast Response"));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingDoesNotExist()
    {
        var response = await _controller.Update(Guid.NewGuid(), new UpdateWayOfWorkingRequest("fast-response"));

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    private async Task<Guid> AddWay()
    {
        var wayId = Guid.NewGuid();
        await _appDbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(wayId));
        await _appDbContext.SaveChangesAsync();

        return wayId;
    }

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
