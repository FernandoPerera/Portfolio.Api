using LanguageExt;
using NSubstitute;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.Dtos;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Application.UseCases;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Application.UseCases;

public class WayTranslationUpdaterShould
{
    private readonly Guid _wayId = Guid.NewGuid();
    private readonly IWayOfWorkingByIdFinderRepository _finderRepository = Substitute.For<IWayOfWorkingByIdFinderRepository>();
    private readonly IWayTranslationUpdaterRepository _updaterRepository = Substitute.For<IWayTranslationUpdaterRepository>();
    private readonly WayTranslationUpdater _updater;

    public WayTranslationUpdaterShould()
    {
        _updater = new WayTranslationUpdater(_finderRepository, _updaterRepository);
        _finderRepository.FindWay(Arg.Is<WayOfWorkingId>(id => id.ToGuid() == _wayId)).Returns(
            WayOfWorkingFoundBuilder.Build(_wayId, translations: [WayTranslationBuilder.Build("es", "Claro", "Sin jerga")])
        );
        _updaterRepository.UpdateTranslation(Arg.Any<WayTranslationToBeUpdated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task UpdateTheTranslation()
    {
        const string givenLabel = "Claro y simple";
        var info = new WayTranslationUpdaterInfo(_wayId, "es", givenLabel, null, null, null, null);

        var result = await _updater.Execute(info);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _updaterRepository.Received(1).UpdateTranslation(Arg.Is<WayTranslationToBeUpdated>(translation =>
                    translation.GetWayId() == _wayId
                    && translation.GetLanguageCode() == "es"
                    && translation.GetLabel() == givenLabel
                    && translation.GetCaption() == "Sin jerga"
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
