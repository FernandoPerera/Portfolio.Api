using LanguageExt;
using NSubstitute;
using Portfolio.Api.Languages.Create.Application.Dtos;
using Portfolio.Api.Languages.Create.Application.UseCases;
using Portfolio.Api.Languages.Create.Domain.Models;
using Portfolio.Api.Languages.Create.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Create.Application.UseCases;

public class LanguageCreatorShould
{
    private readonly ILanguageCreatorRepository _creatorRepository = Substitute.For<ILanguageCreatorRepository>();
    private readonly LanguageCreator _creator;

    public LanguageCreatorShould()
    {
        _creator = new LanguageCreator(_creatorRepository);
    }

    [Fact]
    public async Task CreateLanguage()
    {
        var creatorInfo = new LanguageCreatorInfo(LanguageCode: "es", Name: "Español");
        _creatorRepository.CreateLanguage(Arg.Any<LanguageToBeCreated>()).Returns(Unit.Default);

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }
}
