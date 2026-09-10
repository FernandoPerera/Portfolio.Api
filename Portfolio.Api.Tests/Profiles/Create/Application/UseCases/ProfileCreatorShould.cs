using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Application.UseCases;

public class ProfileCreatorShould
{
    [Fact]
    public async Task CreateProfile()
    {
        var repository = Substitute.For<IProfileCreatorRepository>();
        var creator = new ProfileCreator(repository);
        var creatorInfo = new ProfileCreatorInfo(
            Name: "Fernando",
            Role: "Beer software architect",
            Presentation: "Mucho gusto :)",
            Enterprise: "Lean Mind"
        );
        repository.CreateProfile(Arg.Any<Profile>()).Returns(Unit.Default);

        (
            await creator.Execute(creatorInfo)
        ).Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }
}