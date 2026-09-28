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
        _controller = new ProfileCreatorController(creator);
    }

    [Fact]
    public async Task CreateProfile()
    {
        await _appDbContext.Languages.AddAsync(new LanguageEntity
        {
            LanguageCode = "es",
            Name = "Spanish"
        });
        await _appDbContext.SaveChangesAsync();

        var request = new CreateProfileRequest(
            "Fernando",
            "Beer software architect",
            "Mucho gusto :)",
            "Lean Mind",
            "es"
        );

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task ReturnNotFoundWhenLanguageDoesNotExist()
    {
        var request = new CreateProfileRequest(
            "Fernando",
            "Beer software architect",
            "Mucho gusto :)",
            "Lean Mind",
            "fr"
        );

        var response = await _controller.Create(request);

        response.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ReturnBadRequestWhenLanguageCodeIsInvalid()
    {
        var request = new CreateProfileRequest(
            "Fernando",
            "Beer software architect",
            "Mucho gusto :)",
            "Lean Mind",
            "invalid_code"
        );

        var response = await _controller.Create(request);

        response.ShouldBeOfType<BadRequestObjectResult>();
    }
}