using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Add.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Add.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Add.Infrastructure.Controllers;
using Portfolio.Api.WaysOfWorking.Add.Infrastructure.Dtos;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Infrastructure.Controllers;

public class WayTranslationAdderControllerShould
{
    private const string NewLanguage = "en";
    private const string ExistingLanguage = "es";

    private readonly AppDbContext _appDbContext;
    private readonly WayAdderController _controller;

    public WayTranslationAdderControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        var adder = new WayAdder(
            new EntityFrameworkWayOfWorkingByIdFinderAdapter(_appDbContext),
            new EntityFrameworkLanguageExistenceAdapter(_appDbContext),
            new EntityFrameworkWayAdderAdapter(_appDbContext)
        );
        _controller = new WayAdderController(adder, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task AddTranslation()
    {
        var wayId = await AddWay();
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(wayId, NewLanguage, BuildRequest());

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenTheTranslationIsInvalid()
    {
        var wayId = await AddWay();
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(wayId, NewLanguage, BuildRequest(label: " "));

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingDoesNotExist()
    {
        await AddLanguage(NewLanguage);

        var response = await _controller.Add(Guid.NewGuid(), NewLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheLanguageDoesNotExist()
    {
        var wayId = await AddWay();

        var response = await _controller.Add(wayId, NewLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenTheTranslationAlreadyExists()
    {
        var wayId = await AddWay();

        var response = await _controller.Add(wayId, ExistingLanguage, BuildRequest());

        AssertProblem(response, StatusCodes.Status409Conflict);
    }

    private async Task<Guid> AddWay()
    {
        var wayId = Guid.NewGuid();
        _appDbContext.SeedLanguages(ExistingLanguage);
        await _appDbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(
            wayId,
            translations: [WayOfWorkingTranslationEntityBuilder.Build(wayId, ExistingLanguage)]
        ));
        await _appDbContext.SaveChangesAsync();

        return wayId;
    }

    private async Task AddLanguage(string languageCode)
    {
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "Language" });
        await _appDbContext.SaveChangesAsync();
    }

    private static AddWayRequest BuildRequest(string label = "Clear") =>
        new(label, "No jargon", "I explain it without jargon", "Before writing a line we talk in plain words.", ["A short chat"]);

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
