using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkExpiredRefreshTokenCleanerAdapterShould : IDisposable
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly FakeLogger<EntityFrameworkExpiredRefreshTokenCleanerAdapter> _logger = new();
    private readonly EntityFrameworkExpiredRefreshTokenCleanerAdapter _cleaner;

    public EntityFrameworkExpiredRefreshTokenCleanerAdapterShould()
    {
        _appDbContext = PostgresTestDatabase.CreateContext();
        _cleaner = new EntityFrameworkExpiredRefreshTokenCleanerAdapter(
            _appDbContext,
            new FixedTimeProvider(Now),
            _logger
        );
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
    public async Task RemoveNothingWhenNoRefreshTokenHasExpired()
    {
        var active = RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("active"), expiresAt: Now.AddSeconds(1));
        _appDbContext.RefreshTokens.Add(active);
        await _appDbContext.SaveChangesAsync();

        var result = await _cleaner.Clean();

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.RefreshTokens.ShouldHaveSingleItem().Id.ShouldBe(active.Id);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotFailAndWarnWhenTheCleaningCannotBeDone()
    {
        var unreachable = PostgresTestDatabase.CreateUnreachableContext();
        var cleaner = new EntityFrameworkExpiredRefreshTokenCleanerAdapter(unreachable, new FixedTimeProvider(Now), _logger);

        var result = await cleaner.Clean();

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var entry = _logger.Collector.GetSnapshot().ShouldHaveSingleItem();
                entry.Level.ShouldBe(LogLevel.Warning);
                entry.Exception.ShouldNotBeNull();
            },
            error => error.ShouldBeNull()
        );
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}
