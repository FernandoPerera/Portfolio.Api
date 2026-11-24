using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Update.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Update.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingUpdaterAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayOfWorkingUpdaterAdapter _updater;

    public EntityFrameworkWayOfWorkingUpdaterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _updater = new EntityFrameworkWayOfWorkingUpdaterAdapter(_dbContext);
    }

    [Fact]
    public async Task UpdateOnlyTheIllustrationOfTheGivenWay()
    {
        var wayId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        await _dbContext.WaysOfWorking.AddRangeAsync(
            WayOfWorkingEntityBuilder.Build(wayId, 1, "plain-language"),
            WayOfWorkingEntityBuilder.Build(otherId, 2, "tested")
        );
        await _dbContext.SaveChangesAsync();

        var result = await _updater.UpdateWay(WayOfWorkingToBeUpdatedBuilder.Build(wayId, "fast-response"));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var stored = _dbContext.WaysOfWorking.AsNoTracking().Include(way => way.Translations)
                    .OrderBy(way => way.Position)
                    .ToList();
                stored.Select(way => (way.Id, way.Position, way.Illustration))
                    .ShouldBe([(wayId, 1, "fast-response"), (otherId, 2, "tested")]);
                stored.ShouldAllBe(way => way.Translations.Count == 1);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _updater.UpdateWay(WayOfWorkingToBeUpdatedBuilder.Build());

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
