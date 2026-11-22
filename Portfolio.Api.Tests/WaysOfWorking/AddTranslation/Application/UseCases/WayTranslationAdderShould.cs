using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.AddTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.AddTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.AddTranslation.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Application.UseCases;

public class WayTranslationAdderShould
{
    private readonly Guid _wayId = Guid.NewGuid();
    private readonly IWayOfWorkingByIdFinderRepository _finderRepository = Substitute.For<IWayOfWorkingByIdFinderRepository>();
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly IWayTranslationAdderRepository _adderRepository = Substitute.For<IWayTranslationAdderRepository>();
    private readonly WayTranslationAdder _adder;

    public WayTranslationAdderShould()
    {
        _adder = new WayTranslationAdder(_finderRepository, _languageExistenceRepository, _adderRepository);
        _finderRepository.FindWay(Arg.Is<WayOfWorkingId>(id => id.ToGuid() == _wayId)).Returns(
            WayOfWorkingFoundBuilder.Build(_wayId, translations: [WayTranslationBuilder.Build("es")])
        );
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _adderRepository.AddTranslation(Arg.Any<WayTranslationToBeAdded>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task AddTranslation()
    {
        const string givenLanguageCode = "en";
        const string givenLabel = "Clear";
        var info = new WayTranslationAdderInfo(
            _wayId,
            givenLanguageCode,
            givenLabel,
            "No jargon",
            "I explain it without jargon",
            "Before writing a line we talk in plain words.",
            ["A short chat"]
        );

        var result = await _adder.Execute(info);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _languageExistenceRepository.Received(1).Exists(Arg.Any<LanguageCode>());
                _adderRepository.Received(1).AddTranslation(Arg.Is<WayTranslationToBeAdded>(translation =>
                    translation.GetWayId() == _wayId
                    && translation.GetTranslation().GetLanguageCode() == givenLanguageCode
                    && translation.GetTranslation().GetLabel() == givenLabel
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
