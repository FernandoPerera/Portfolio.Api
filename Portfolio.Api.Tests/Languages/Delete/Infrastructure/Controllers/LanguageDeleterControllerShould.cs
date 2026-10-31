using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Delete.Application.UseCases;
using Portfolio.Api.Languages.Delete.Infrastructure.Adapters;
using Portfolio.Api.Languages.Delete.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Delete.Infrastructure.Controllers;

public class LanguageDeleterControllerShould
{
    private const string LanguageCode = "es";

    private readonly AppDbContext _appDbContext;
    private readonly LanguageDeleterController _controller;

    public LanguageDeleterControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var deleter = new LanguageDeleter(
            new EntityFrameworkLanguageExistenceAdapter(_appDbContext),
            new EntityFrameworkLanguageUsageAdapter(_appDbContext),
            new EntityFrameworkLanguageDeleterAdapter(_appDbContext)
        );
        _controller = new LanguageDeleterController(deleter, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task DeleteLanguage()
    {
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = LanguageCode, Name = "Spanish" });
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Delete(LanguageCode);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenLanguageCodeIsInvalid()
    {
        var response = await _controller.Delete("invalid_code");

        AssertProblem(response, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var response = await _controller.Delete(LanguageCode);

        AssertProblem(response, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenLanguageIsInUse()
    {
        var profileId = Guid.NewGuid();
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = LanguageCode, Name = "Spanish" });
        await _appDbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(
            profileId,
            translations: [ProfileTranslationEntityBuilder.Build(profileId, LanguageCode)]
        ));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Delete(LanguageCode);

        AssertProblem(response, StatusCodes.Status409Conflict);
    }

    private static void AssertProblem(ActionResult response, int statusCode)
    {
        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(statusCode);
    }
}
