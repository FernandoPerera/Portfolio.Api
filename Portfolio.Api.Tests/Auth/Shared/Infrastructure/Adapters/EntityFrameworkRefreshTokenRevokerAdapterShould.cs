using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenRevokerAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkRefreshTokenRevokerAdapter _revoker;

    public EntityFrameworkRefreshTokenRevokerAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _revoker = new EntityFrameworkRefreshTokenRevokerAdapter(_appDbContext);
    }

    [Fact]
    public async Task RevokeAllTheRefreshTokensOfTheOwner()
    {
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("first"), owner: "admin"));
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("second"), owner: "admin"));
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(RefreshTokenBuilder.Build("other"), owner: "other-admin"));
        await _appDbContext.SaveChangesAsync();

        var result = await _revoker.Revoke(StoredRefreshTokenBuilder.Build(Now.AddDays(1), owner: "admin"));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.RefreshTokens.ShouldHaveSingleItem().Owner.ShouldBe("other-admin");
            },
            error => error.ShouldBeNull()
        );
    }
}
