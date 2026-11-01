using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Update.Application.UseCases;
using Portfolio.Api.Languages.Update.Infrastructure.Adapters;
using Portfolio.Api.Languages.Update.Infrastructure.Controllers;
using Portfolio.Api.Languages.Update.Infrastructure.Dtos;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Update.Infrastructure.Controllers;

public class LanguageUpdaterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly LanguageUpdaterController _controller;

    public LanguageUpdaterControllerShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        var updater = new LanguageUpdater(
            new EntityFrameworkLanguageExistenceAdapter(_appDbContext),
            new EntityFrameworkLanguageUpdaterAdapter(_appDbContext)
        );
        _controller = new LanguageUpdaterController(updater, ProblemDetailsMapperBuilder.Build());
    }

    [Fact]
    public async Task UpdateLanguage()
    {
        const string languageCode = "es";
        await _appDbContext.Languages.AddAsync(new LanguageEntity { LanguageCode = languageCode, Name = "Spanish" });
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Update(languageCode, new UpdateLanguageRequest("Español"));

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenNameIsInvalid()
    {
        var response = await _controller.Update("es", new UpdateLanguageRequest(" "));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var response = await _controller.Update("fr", new UpdateLanguageRequest("Français"));

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
}
