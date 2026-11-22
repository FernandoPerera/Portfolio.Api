using LanguageExt;
using NSubstitute;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Update.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Update.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Update.Domain.Models;
using Portfolio.Api.WaysOfWorking.Update.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Update.Application.UseCases;

public class WayOfWorkingUpdaterShould
{
    private readonly Guid _wayId = Guid.NewGuid();
    private readonly IWayOfWorkingByIdFinderRepository _finderRepository = Substitute.For<IWayOfWorkingByIdFinderRepository>();
    private readonly IWayOfWorkingUpdaterRepository _updaterRepository = Substitute.For<IWayOfWorkingUpdaterRepository>();
    private readonly WayOfWorkingUpdater _updater;

    public WayOfWorkingUpdaterShould()
    {
        _updater = new WayOfWorkingUpdater(_finderRepository, _updaterRepository);
        _finderRepository.FindWay(Arg.Is<WayOfWorkingId>(id => id.ToGuid() == _wayId))
            .Returns(WayOfWorkingFoundBuilder.Build(_wayId));
        _updaterRepository.UpdateWay(Arg.Any<WayOfWorkingToBeUpdated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task UpdateTheIllustration()
    {
        const string givenIllustration = "fast-response";

        var result = await _updater.Execute(new WayOfWorkingUpdaterInfo(_wayId, givenIllustration));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _updaterRepository.Received(1).UpdateWay(Arg.Is<WayOfWorkingToBeUpdated>(way =>
                    way.GetId() == _wayId && way.GetIllustration() == givenIllustration
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
