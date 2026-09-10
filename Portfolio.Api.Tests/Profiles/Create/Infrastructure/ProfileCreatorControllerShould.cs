using Microsoft.AspNetCore.Http.HttpResults;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Create.Infrastructure.Controllers;
using Portfolio.Api.Profiles.Create.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Infrastructure;

public class ProfileCreatorControllerShould
{
    [Fact]
    public async Task CreateProfile()
    {
        var repository = new SqlServerProfileCreatorAdapter();
        var creator = new ProfileCreator(repository);
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