using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Create.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Infrastructure.Controllers;

public class ProfileCreatorControllerShould
{
    private readonly ProfileCreatorController _controller;

    public ProfileCreatorControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "example_db")
            .Options;

        var generatorIdentifier = new GuidGeneratorIdentifier();
        var repository = new EntityFrameworkProfileCreatorAdapter(new AppDbContext(options), generatorIdentifier);
        var creator = new ProfileCreator(repository);
        _controller = new ProfileCreatorController(creator);
    }
    
    [Fact]
    public async Task CreateProfile()
    {
        var request = new CreateProfileRequest(
            "Fernando",
            "Beer software architect",
            "Mucho gusto :)",
            "Lean Mind"
        );

        var response = await _controller.Create(request);

        response.ShouldBeOfType<CreatedAtActionResult>();
    }
}