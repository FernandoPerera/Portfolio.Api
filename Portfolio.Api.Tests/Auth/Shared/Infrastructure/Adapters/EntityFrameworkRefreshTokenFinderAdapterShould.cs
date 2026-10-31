using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenFinderAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly Sha256RefreshTokenHasher _hasher = new();
    private readonly EntityFrameworkRefreshTokenFinderAdapter _finder;

    public EntityFrameworkRefreshTokenFinderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        _finder = new EntityFrameworkRefreshTokenFinderAdapter(_appDbContext);
    }

    [Fact]
    public async Task FindTheStoredRefreshTokenByItsHash()
    {
        var refreshToken = RefreshTokenBuilder.Build("refresh-token");
        var entity = RefreshTokenEntityBuilder.Build(refreshToken, owner: "admin", expiresAt: Now.AddDays(1));
        _appDbContext.RefreshTokens.Add(entity);
        await _appDbContext.SaveChangesAsync();

        var result = await _finder.Find(_hasher.Hash(refreshToken).IfLeft(error => throw new Exception(error.Message)));

        result.Match(
            stored =>
            {
                stored.GetId().ShouldBe(entity.Id);
                stored.GetTokenHash().ShouldBe(entity.TokenHash);
                stored.GetOwner().ShouldBe("admin");
                stored.GetExpiresAt().ShouldBe(Now.AddDays(1));
                stored.GetConsumedAt().ShouldBeNull();
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RejectAnUnknownRefreshToken()
    {
        var result = await _finder.Find(RefreshTokenHashBuilder.Build("unknown"));

        result.Match(
            stored => stored.ShouldBeNull(),
            error => error.ShouldBeOfType<UnauthorizedError>()
                .Message.ShouldBe("Invalid refresh token")
        );
    }
}
