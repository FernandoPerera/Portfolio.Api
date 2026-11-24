using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Infrastructure.Controllers;

public class WayTranslationUpdaterControllerShould
{
    private const string ExistingLanguage = "es";

    private readonly AppDbContext _appDbContext;
    private readonly WayTranslationUpdaterController _controller;

    public WayTranslationUpdaterControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages(ExistingLanguage, "en");
        var updater = new WayTranslationUpdater(
            new EntityFrameworkWayOfWorkingByIdFinderAdapter(_appDbContext),
            new EntityFrameworkWayTranslationUpdaterAdapter(_appDbContext)
        );
        _controller = new WayTranslationUpdaterController(updater, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task UpdateTheTranslation()
    {
        var wayId = await AddWay();

        var response = await _controller.Update(wayId, ExistingLanguage, new UpdateWayTranslationRequest("Claro y simple", null, null, null, null));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenAFieldIsInvalid()
    {
        var wayId = await AddWay();

        var response = await _controller.Update(wayId, ExistingLanguage, new UpdateWayTranslationRequest(" ", null, null, null, null));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingDoesNotExist()
    {
        var response = await _controller.Update(Guid.NewGuid(), ExistingLanguage, new UpdateWayTranslationRequest("Claro", null, null, null, null));

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingHasNoTranslationInTheLanguage()
    {
        var wayId = await AddWay();

        var response = await _controller.Update(wayId, "en", new UpdateWayTranslationRequest("Clear", null, null, null, null));

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    private async Task<Guid> AddWay()
    {
        var wayId = Guid.NewGuid();
        await _appDbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(
            wayId,
            translations: [WayOfWorkingTranslationEntityBuilder.Build(wayId, ExistingLanguage)]
        ));
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
