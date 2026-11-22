using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Reorder.Infrastructure.Adapters;

public class EntityFrameworkWayOrderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayOrderAdapter _orderAdapter;

    public EntityFrameworkWayOrderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es");
        _orderAdapter = new EntityFrameworkWayOrderAdapter(_dbContext);
    }

    [Fact]
    public async Task ReorderTheWaysOfWorking()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var third = Guid.NewGuid();
        await _dbContext.WaysOfWorking.AddRangeAsync(
            WayOfWorkingEntityBuilder.Build(first, 1),
            WayOfWorkingEntityBuilder.Build(second, 2),
            WayOfWorkingEntityBuilder.Build(third, 3)
        );
        await _dbContext.SaveChangesAsync();
        var order = WayOrder.Create([third, first, second]).IfLeft(error => throw new Exception(error.Message));

        var result = await _orderAdapter.ReorderWays(order);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.WaysOfWorking.AsNoTracking().OrderBy(way => way.Position)
                    .Select(way => way.Id)
                    .ToList()
                    .ShouldBe([third, first, second]);
                _dbContext.WaysOfWorking.AsNoTracking().OrderBy(way => way.Position)
                    .Select(way => way.Position)
                    .ToList()
                    .ShouldBe([1, 2, 3]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var order = WayOrder.Create([Guid.NewGuid()]).IfLeft(error => throw new Exception(error.Message));
        await _dbContext.DisposeAsync();

        var result = await _orderAdapter.ReorderWays(order);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
