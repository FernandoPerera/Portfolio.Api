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
    private readonly IProfileCreatorRepository _repository;
    private readonly ProfileCreator _creator;

    public ProfileCreatorShould()
    {
        _repository = Substitute.For<IProfileCreatorRepository>();
        _creator = new ProfileCreator(_repository);
    }

    [Fact]
    public async Task CreateProfile()
    {
        var creatorInfo = new ProfileCreatorInfo(
            Name: "Fernando",
            JobRole: "Beer software architect",
            Presentation: "Mucho gusto :)",
            EnterpriseName: "Lean Mind"
        );
        _repository.CreateProfile(Arg.Any<ProfileToBeCreated>()).Returns(Unit.Default);

        (
            await _creator.Execute(creatorInfo)
        ).Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }
}