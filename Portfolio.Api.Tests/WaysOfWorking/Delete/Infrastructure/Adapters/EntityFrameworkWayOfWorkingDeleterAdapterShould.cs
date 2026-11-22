using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Delete.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingDeleterAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayOfWorkingDeleterAdapter _deleter;

    public EntityFrameworkWayOfWorkingDeleterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _deleter = new EntityFrameworkWayOfWorkingDeleterAdapter(_dbContext);
    }

    [Fact]
    public async Task DeleteTheWayOfWorkingAndCloseTheGapInThePositions()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var third = Guid.NewGuid();
        var fourth = Guid.NewGuid();
        await _dbContext.WaysOfWorking.AddRangeAsync(
            WayOfWorkingEntityBuilder.Build(first, 1),
            WayOfWorkingEntityBuilder.Build(second, 2, translations:
            [
                WayOfWorkingTranslationEntityBuilder.Build(second, "es"),
                WayOfWorkingTranslationEntityBuilder.Build(second, "en")
            ]),
            WayOfWorkingEntityBuilder.Build(third, 3),
            WayOfWorkingEntityBuilder.Build(fourth, 4)
        );
        await _dbContext.SaveChangesAsync();

        var result = await _deleter.DeleteWay(WayOfWorkingIdBuilder.Build(second));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.WaysOfWorking.AsNoTracking().OrderBy(way => way.Position)
                    .Select(way => new { way.Id, way.Position })
                    .ToList()
                    .ShouldBe([new { Id = first, Position = 1 }, new { Id = third, Position = 2 }, new { Id = fourth, Position = 3 }]);
                _dbContext.WayOfWorkingTranslations.AsNoTracking().Count(translation => translation.WayId == second)
                    .ShouldBe(0);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _deleter.DeleteWay(WayOfWorkingIdBuilder.Build());

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
