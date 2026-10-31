using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenRevokerAdapterShould : IDisposable
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkRefreshTokenRevokerAdapter _revoker;

    public EntityFrameworkRefreshTokenRevokerAdapterShould()
    {
        _appDbContext = PostgresTestDatabase.CreateContext();
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

    [Fact]
    public async Task RevokeNothingWhenTheOwnerHasNoRefreshTokens()
    {
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

    [Fact]
    public async Task FailWithServiceUnavailableWhenTheRevocationCannotBeDone()
    {
        var revoker = new EntityFrameworkRefreshTokenRevokerAdapter(PostgresTestDatabase.CreateUnreachableContext());

        var result = await revoker.Revoke(StoredRefreshTokenBuilder.Build(Now.AddDays(1), owner: "admin"));

        result.Match(
            _ => Assert.Fail("Expected a ServiceUnavailableError"),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}
