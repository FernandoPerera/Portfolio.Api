using NSubstitute;
using Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Application.UseCases;

public class WayOfWorkingFinderShould
{
    private readonly IWayOfWorkingFinderRepository _repository;
    private readonly WayOfWorkingFinder _finder;

    public WayOfWorkingFinderShould()
    {
        _repository = Substitute.For<IWayOfWorkingFinderRepository>();
        _finder = new WayOfWorkingFinder(_repository);
    }

    [Fact]
    public async Task FindWaysOfWorking()
    {
        var ways = new List<WayOfWorkingFound>
        {
            WayOfWorkingFoundBuilder.Build(position: 1),
            WayOfWorkingFoundBuilder.Build(position: 2, illustration: "tested")
        };
        _repository.FindWays().Returns(ways);

        var result = await _finder.Execute();

        result.Match(
            found => found.ShouldBe(ways),
            error => error.ShouldBeNull()
        );
    }
}
