using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Delete.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Delete.Infrastructure.Controllers;

public class WayOfWorkingDeleterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly WayOfWorkingDeleterController _controller;

    public WayOfWorkingDeleterControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages("es");
        var deleter = new WayOfWorkingDeleter(
            new EntityFrameworkWayOfWorkingByIdFinderAdapter(_appDbContext),
            new EntityFrameworkWayOfWorkingDeleterAdapter(_appDbContext)
        );
        _controller = new WayOfWorkingDeleterController(deleter, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task DeleteTheWayOfWorking()
    {
        var wayId = Guid.NewGuid();
        await _appDbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(wayId));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Delete(wayId);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingDoesNotExist()
    {
        var response = await _controller.Delete(Guid.NewGuid());

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
}
