using LanguageExt;
using NSubstitute;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Delete.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Delete.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Delete.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Delete.Application.UseCases;

public class WayOfWorkingDeleterShould
{
    private readonly Guid _wayId = Guid.NewGuid();
    private readonly IWayOfWorkingByIdFinderRepository _finderRepository = Substitute.For<IWayOfWorkingByIdFinderRepository>();
    private readonly IWayOfWorkingDeleterRepository _deleterRepository = Substitute.For<IWayOfWorkingDeleterRepository>();
    private readonly WayOfWorkingDeleter _deleter;

    public WayOfWorkingDeleterShould()
    {
        _deleter = new WayOfWorkingDeleter(_finderRepository, _deleterRepository);
        _finderRepository.FindWay(Arg.Is<WayOfWorkingId>(id => id.ToGuid() == _wayId))
            .Returns(WayOfWorkingFoundBuilder.Build(_wayId));
        _deleterRepository.DeleteWay(Arg.Any<WayOfWorkingId>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task DeleteTheWayOfWorking()
    {
        var result = await _deleter.Execute(new WayOfWorkingDeleterInfo(_wayId));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _deleterRepository.Received(1).DeleteWay(Arg.Is<WayOfWorkingId>(id => id.ToGuid() == _wayId));
            },
            error => error.ShouldBeNull()
        );
    }
}
