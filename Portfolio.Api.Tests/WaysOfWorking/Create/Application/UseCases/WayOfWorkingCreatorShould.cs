using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Create.Application.Builders;
using Portfolio.Api.WaysOfWorking.Create.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;
using Portfolio.Api.WaysOfWorking.Create.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Application.UseCases;

public class WayOfWorkingCreatorShould
{
    private readonly IWayOfWorkingCreatorRepository _creatorRepository = Substitute.For<IWayOfWorkingCreatorRepository>();
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly WayOfWorkingCreator _creator;

    public WayOfWorkingCreatorShould()
    {
        _creator = new WayOfWorkingCreator(_creatorRepository, _languageExistenceRepository);
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _creatorRepository.CreateWay(Arg.Any<WayOfWorkingToBeCreated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task CreateWayOfWorking()
    {
        var creatorInfo = WayOfWorkingCreatorInfoBuilder.Build(
            translations:
            [
                WayTranslationInfoBuilder.Build("es"),
                WayTranslationInfoBuilder.Build("en")
            ]
        );

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _languageExistenceRepository.Received(2).Exists(Arg.Any<LanguageCode>());
                _creatorRepository.Received(1).CreateWay(Arg.Any<WayOfWorkingToBeCreated>());
            },
            error => error.ShouldBeNull()
        );
    }
}
