using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Reorder.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Reorder.Infrastructure.Controllers;

public class WayOrderUpdaterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly WayOrderUpdaterController _controller;
    private readonly Guid _first = Guid.NewGuid();
    private readonly Guid _second = Guid.NewGuid();

    public WayOrderUpdaterControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages("es");
        var updater = new WayOrderUpdater(
            new EntityFrameworkWayOfWorkingFinderAdapter(_appDbContext),
            new EntityFrameworkWayOrderAdapter(_appDbContext)
        );
        _controller = new WayOrderUpdaterController(updater, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task ReorderTheWaysOfWorking()
    {
        await AddWays();

        var response = await _controller.Reorder(new ReorderWaysOfWorkingRequest([_second, _first]));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenTheOrderIsInvalid()
    {
        await AddWays();

        var response = await _controller.Reorder(new ReorderWaysOfWorkingRequest([_first, _first]));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheOrderLeavesAWayOfWorkingOut()
    {
        await AddWays();

        var response = await _controller.Reorder(new ReorderWaysOfWorkingRequest([_first]));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    private async Task AddWays()
    {
        await _appDbContext.WaysOfWorking.AddRangeAsync(
            WayOfWorkingEntityBuilder.Build(_first, 1),
            WayOfWorkingEntityBuilder.Build(_second, 2)
        );
        await _appDbContext.SaveChangesAsync();
    }

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
