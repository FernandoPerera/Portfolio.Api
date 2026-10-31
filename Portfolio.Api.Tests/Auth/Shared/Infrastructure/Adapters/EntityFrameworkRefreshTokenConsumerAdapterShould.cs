using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenConsumerAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly DbContextOptions<AppDbContext> _options = PostgresTestDatabase.CreateOptions();

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkRefreshTokenConsumerAdapter _consumer;

    public EntityFrameworkRefreshTokenConsumerAdapterShould()
    {
        _appDbContext = new AppDbContext(_options);
        _consumer = new EntityFrameworkRefreshTokenConsumerAdapter(_appDbContext);
    }

    [Fact]
    public async Task ConsumeTheStoredRefreshToken()
    {
        var entity = RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build(), expiresAt: Now.AddDays(1));
        _appDbContext.RefreshTokens.Add(entity);
        await _appDbContext.SaveChangesAsync();

        var result = await _consumer.Consume(
            StoredRefreshTokenBuilder.Build(Now.AddDays(1), consumedAt: Now, id: entity.Id)
        );

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.RefreshTokens.ShouldHaveSingleItem().ConsumedAt.ShouldBe(Now);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RejectAnUnknownRefreshToken()
    {
        var result = await _consumer.Consume(StoredRefreshTokenBuilder.Build(Now.AddDays(1), consumedAt: Now));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<UnauthorizedError>()
                .Message.ShouldBe("Invalid refresh token")
        );
    }

    [Fact]
    public async Task RejectTheSecondConsumptionOfTheSameRefreshToken()
    {
        var entity = RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build(), expiresAt: Now.AddDays(1));
        _appDbContext.RefreshTokens.Add(entity);
        await _appDbContext.SaveChangesAsync();
        var consumed = StoredRefreshTokenBuilder.Build(Now.AddDays(1), consumedAt: Now, id: entity.Id);
        var concurrentConsumer = new EntityFrameworkRefreshTokenConsumerAdapter(new AppDbContext(_options));
        await concurrentConsumer.Consume(consumed);

        var result = await _consumer.Consume(consumed);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<RefreshTokenReuseError>()
                .Message.ShouldBe("Refresh token has already been used")
        );
    }

    [Fact]
    public async Task RejectAsUnknownWhenTheRefreshTokenWasRevokedMeanwhile()
    {
        var entity = RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build(), expiresAt: Now.AddDays(1));
        _appDbContext.RefreshTokens.Add(entity);
        await _appDbContext.SaveChangesAsync();
        var concurrentContext = new AppDbContext(_options);
        concurrentContext.RefreshTokens.Remove(await concurrentContext.RefreshTokens.FindAsync(entity.Id) ?? entity);
        await concurrentContext.SaveChangesAsync();

        var result = await _consumer.Consume(
            StoredRefreshTokenBuilder.Build(Now.AddDays(1), consumedAt: Now, id: entity.Id)
        );

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<UnauthorizedError>()
                .Message.ShouldBe("Invalid refresh token")
        );
    }
}
