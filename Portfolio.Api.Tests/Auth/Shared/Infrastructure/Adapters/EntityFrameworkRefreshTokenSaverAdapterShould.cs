using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenSaverAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly IGeneratorIdentifier _idGenerator = Substitute.For<IGeneratorIdentifier>();
    private readonly EntityFrameworkRefreshTokenSaverAdapter _saver;

    public EntityFrameworkRefreshTokenSaverAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _saver = new EntityFrameworkRefreshTokenSaverAdapter(
            _appDbContext,
            _idGenerator,
            new Sha256RefreshTokenHasher(),
            new JwtOptions { RefreshTokenDays = 7 },
            new FixedTimeProvider(Now)
        );
    }

    [Fact]
    public async Task SaveTheHashOfTheRefreshTokenWithItsOwnerAndExpiration()
    {
        var tokenId = Guid.NewGuid();
        _idGenerator.Generate().Returns(tokenId);
        var refreshToken = RefreshTokenBuilder.Build("refresh-token");
        var expected = RefreshTokenEntityBuilder.Build(
            refreshToken,
            owner: "admin",
            expiresAt: Now.AddDays(7),
            id: tokenId
        );

        var result = await _saver.Save(refreshToken, AdminUsernameBuilder.Build("admin"));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.RefreshTokens.ShouldHaveSingleItem().ShouldBeEquivalentTo(expected);
            },
            error => error.ShouldBeNull()
        );
    }
}
