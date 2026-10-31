using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Create.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Infrastructure.Controllers;

public class ProfileCreatorControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly ProfileCreatorController _controller;

    public ProfileCreatorControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var generatorIdentifier = new GuidGeneratorIdentifier();
        var creatorRepository = new EntityFrameworkProfileCreatorAdapter(_appDbContext, generatorIdentifier);
        var languageFinderRepository = new EntityFrameworkLanguageExistenceAdapter(_appDbContext);
        var creator = new ProfileCreator(creatorRepository, languageFinderRepository);
        var errorResponseMapper = ProblemDetailsMapperBuilder.Build();
        _controller = new ProfileCreatorController(creator, errorResponseMapper);
    }

    [Fact]
    public async Task CreateProfile()
    {
        var request = CreateProfileRequestBuilder.Build();
        await _appDbContext.Languages.AddAsync(new LanguageEntity
        {
            LanguageCode = request.LanguageCode,
            Name = "Spanish"
        });
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var request = CreateProfileRequestBuilder.Build(languageCode: "fr");

        var response = await _controller.Create(request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task FailWhenLanguageCodeIsInvalid()
    {
        var request = CreateProfileRequestBuilder.Build(languageCode: "invalid_code");

        var response = await _controller.Create(request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
}