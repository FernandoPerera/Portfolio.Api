using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkExpiredRefreshTokenCleanerAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkExpiredRefreshTokenCleanerAdapter _cleaner;

    public EntityFrameworkExpiredRefreshTokenCleanerAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _cleaner = new EntityFrameworkExpiredRefreshTokenCleanerAdapter(_appDbContext, new FixedTimeProvider(Now));
    }

    [Fact]
    public async Task RemoveOnlyTheExpiredRefreshTokens()
    {
        var active = RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("active"), expiresAt: Now.AddSeconds(1));
        var consumed = RefreshTokenEntityBuilder.Build(
            RefreshTokenBuilder.Build("consumed"),
            expiresAt: Now.AddDays(1),
            consumedAt: Now.AddMinutes(-5)
        );
        _appDbContext.RefreshTokens.Add(active);
        _appDbContext.RefreshTokens.Add(consumed);
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("expired"), expiresAt: Now));
        _appDbContext.RefreshTokens.Add(
            RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("long-expired"), expiresAt: Now.AddDays(-3))
        );
        await _appDbContext.SaveChangesAsync();

        var result = await _cleaner.Clean();

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.RefreshTokens.Select(token => token.Id).ToList()
                    .ShouldBe([active.Id, consumed.Id], ignoreOrder: true);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotFailAndDiscardTheDeletionsWhenTheCleaningCannotBeDone()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var failingContext = new FailingSaveDbContext(options);
        failingContext.RefreshTokens.Add(
            RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("expired"), expiresAt: Now.AddDays(-1))
        );
        var cleaner = new EntityFrameworkExpiredRefreshTokenCleanerAdapter(failingContext, new FixedTimeProvider(Now));

        var result = await cleaner.Clean();

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                failingContext.ChangeTracker.Entries().ShouldBeEmpty();
            },
            error => error.ShouldBeNull()
        );
    }
}
