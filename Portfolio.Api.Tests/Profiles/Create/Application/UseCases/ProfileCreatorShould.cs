using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Profiles.Create.Application.Dtos;
using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Profiles.Create.Application.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Application.UseCases;

public class ProfileCreatorShould
{
    private readonly IProfileCreatorRepository _creatorRepository = Substitute.For<IProfileCreatorRepository>();
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly ProfileCreator _creator;

    public ProfileCreatorShould()
    {
        _creator = new ProfileCreator(_creatorRepository, _languageExistenceRepository);
    }

    [Fact]
    public async Task CreateProfile()
    {
        var creatorInfo = ProfileCreatorInfoBuilder.Build();
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _creatorRepository.CreateProfile(Arg.Any<ProfileToBeCreated>()).Returns(Unit.Default);

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var creatorInfo = ProfileCreatorInfoBuilder.Build(languageCode: "fr");
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>())
            .Returns(Error.New(new NotFoundException("Language 'fr' was not found")));

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
        );
    }

    [Fact]
    public async Task FailWhenDatabaseUnavailableOnLanguageCheck()
    {
        var creatorInfo = ProfileCreatorInfoBuilder.Build();
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>())
            .Returns(Error.New(new ServiceUnavailableException("Database is unavailable")));

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}