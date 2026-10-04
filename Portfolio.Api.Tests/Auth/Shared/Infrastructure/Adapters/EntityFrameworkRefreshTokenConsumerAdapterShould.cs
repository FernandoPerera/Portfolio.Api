using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenConsumerAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkRefreshTokenConsumerAdapter _consumer;

    public EntityFrameworkRefreshTokenConsumerAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _consumer = new EntityFrameworkRefreshTokenConsumerAdapter(_appDbContext, new FixedTimeProvider(Now));
    }

    [Fact]
    public async Task ConsumeAValidRefreshTokenAndReturnItsOwner()
    {
        var refreshToken = RefreshTokenBuilder.Build("refresh-token");
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(refreshToken, owner: "admin", expiresAt: Now.AddDays(1)));
        await _appDbContext.SaveChangesAsync();

        var result = await _consumer.Consume(refreshToken);

        result.Match(
            owner =>
            {
                owner.ToString().ShouldBe("admin");
                _appDbContext.RefreshTokens.ShouldHaveSingleItem().ConsumedAt.ShouldBe(Now);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RejectAnUnknownRefreshToken()
    {
        var result = await _consumer.Consume(RefreshTokenBuilder.Build("unknown"));

        result.Match(
            owner => owner.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<UnauthorizedException>()
                .Message.ShouldBe("Invalid refresh token")
        );
    }

    [Fact]
    public async Task RejectAnExpiredRefreshToken()
    {
        var refreshToken = RefreshTokenBuilder.Build("expired");
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(refreshToken, expiresAt: Now.AddSeconds(-1)));
        await _appDbContext.SaveChangesAsync();

        var result = await _consumer.Consume(refreshToken);

        result.Match(
            owner => owner.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<UnauthorizedException>()
        );
    }

    [Fact]
    public async Task RevokeTheWholeSessionWhenARefreshTokenIsReused()
    {
        var reused = RefreshTokenBuilder.Build("reused");
        var latest = RefreshTokenBuilder.Build("latest");
        _appDbContext.RefreshTokens.Add(
            RefreshTokenEntityBuilder.Build(reused, expiresAt: Now.AddDays(1), consumedAt: Now.AddMinutes(-5))
        );
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(latest, expiresAt: Now.AddDays(1)));
        await _appDbContext.SaveChangesAsync();

        var result = await _consumer.Consume(reused);

        result.Match(
            owner => owner.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<UnauthorizedException>();
                _appDbContext.RefreshTokens.ShouldBeEmpty();
            }
        );
    }
}
