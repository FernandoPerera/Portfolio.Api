using Microsoft.AspNetCore.Http.HttpResults;
using Portfolio.Api.Profile.Create.Application.UseCases;
using Portfolio.Api.Profile.Create.Infrastructure.Controllers;
using Portfolio.Api.Profile.Create.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Profile.Create.Infrastructure;

public class ProfileCreatorControllerShould
{
    [Fact]
    public async Task CreateProfile()
    {
        var creator = new ProfileCreator();
        var controller = new ProfileCreatorController(creator);
        var request = new CreateProfileRequest(
            "Fernando",
            "Beer software architect",
            "Mucho gusto :)",
            "Lean Mind"
        );

        var response = await controller.Create(request);

        response.ShouldBeOfType<Created>();
    }
}