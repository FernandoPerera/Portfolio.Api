using LanguageExt;
using NSubstitute;
using Portfolio.Api.Auth.Logout.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Logout.Application.Builders;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Logout.Application.UseCases;

public class SessionCloserShould
{
    private const string RefreshTokenValue = "current-refresh-token";
    private const string TokenHashValue = "current-token-hash";

    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly IRefreshTokenHasher _refreshTokenHasher = Substitute.For<IRefreshTokenHasher>();
    private readonly IRefreshTokenFinderRepository _refreshTokenFinder = Substitute.For<IRefreshTokenFinderRepository>();
    private readonly IRefreshTokenRevokerRepository _refreshTokenRevoker = Substitute.For<IRefreshTokenRevokerRepository>();
    private readonly SessionCloser _closer;

    public SessionCloserShould()
    {
        _closer = new SessionCloser(_refreshTokenHasher, _refreshTokenFinder, _refreshTokenRevoker);
        _refreshTokenHasher.Hash(Arg.Any<RefreshToken>()).Returns(RefreshTokenHashBuilder.Build(TokenHashValue));
        _refreshTokenRevoker.Revoke(Arg.Any<StoredRefreshToken>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task CloseTheSessionOfTheRefreshToken()
    {
        var stored = StoredRefreshTokenBuilder.Build(Now.AddDays(1));
        _refreshTokenFinder.Find(Arg.Any<RefreshTokenHash>()).Returns(stored);

        var result = await _closer.Execute(LogoutInfoBuilder.Build(RefreshTokenValue));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _refreshTokenFinder.Received(1).Find(
                    Arg.Is<RefreshTokenHash>(hash => hash.ToString() == TokenHashValue)
                );
                _refreshTokenRevoker.Received(1).Revoke(stored);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task CloseNothingWhenTheRefreshTokenIsUnknown()
    {
        _refreshTokenFinder.Find(Arg.Any<RefreshTokenHash>())
            .Returns(new UnauthorizedError("Invalid refresh token"));

        var result = await _closer.Execute(LogoutInfoBuilder.Build(RefreshTokenValue));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _refreshTokenRevoker.DidNotReceive().Revoke(Arg.Any<StoredRefreshToken>());
            },
            error => error.ShouldBeNull()
        );
    }
}
