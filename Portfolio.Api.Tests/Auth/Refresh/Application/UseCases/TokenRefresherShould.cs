using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Refresh.Application.Builders;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Refresh.Application.UseCases;

public class TokenRefresherShould
{
    private const string Username = "admin";
    private const string CurrentRefreshTokenValue = "current-refresh-token";
    private const string CurrentTokenHashValue = "current-token-hash";
    private const string NewAccessTokenValue = "new-access-token";
    private const string NewRefreshTokenValue = "new-refresh-token";

    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly IRefreshTokenHasher _refreshTokenHasher = Substitute.For<IRefreshTokenHasher>();
    private readonly IRefreshTokenFinderRepository _refreshTokenFinder = Substitute.For<IRefreshTokenFinderRepository>();
    private readonly IRefreshTokenConsumerRepository _refreshTokenConsumer = Substitute.For<IRefreshTokenConsumerRepository>();
    private readonly IRefreshTokenRevokerRepository _refreshTokenRevoker = Substitute.For<IRefreshTokenRevokerRepository>();
    private readonly IAccessTokenIssuer _accessTokenIssuer = Substitute.For<IAccessTokenIssuer>();
    private readonly IRefreshTokenIssuer _refreshTokenIssuer = Substitute.For<IRefreshTokenIssuer>();
    private readonly IRefreshTokenSaverRepository _refreshTokenSaver = Substitute.For<IRefreshTokenSaverRepository>();
    private readonly TokenRefresher _refresher;

    public TokenRefresherShould()
    {
        _refresher = new TokenRefresher(
            _refreshTokenHasher,
            _refreshTokenFinder,
            _refreshTokenConsumer,
            _refreshTokenRevoker,
            _accessTokenIssuer,
            _refreshTokenIssuer,
            _refreshTokenSaver,
            new FixedTimeProvider(Now)
        );
        _refreshTokenHasher.Hash(Arg.Any<RefreshToken>()).Returns(RefreshTokenHashBuilder.Build(CurrentTokenHashValue));
        _refreshTokenFinder.Find(Arg.Any<RefreshTokenHash>())
            .Returns(StoredRefreshTokenBuilder.Build(Now.AddDays(1), owner: Username));
        _refreshTokenConsumer.Consume(Arg.Any<StoredRefreshToken>()).Returns(Unit.Default);
        _refreshTokenRevoker.Revoke(Arg.Any<StoredRefreshToken>()).Returns(Unit.Default);
        _accessTokenIssuer.Issue(Arg.Any<AdminUsername>()).Returns(AccessTokenBuilder.Build(NewAccessTokenValue));
        _refreshTokenIssuer.Issue().Returns(RefreshTokenBuilder.Build(NewRefreshTokenValue));
        _refreshTokenSaver.Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task RotateTheRefreshTokenAndIssueNewTokens()
    {
        var result = await _refresher.Execute(RefreshInfoBuilder.Build(CurrentRefreshTokenValue));

        result.Match(
            tokens =>
            {
                tokens.GetAccessToken().ShouldBe(NewAccessTokenValue);
                tokens.GetRefreshToken().ShouldBe(NewRefreshTokenValue);
                _refreshTokenFinder.Received(1).Find(
                    Arg.Is<RefreshTokenHash>(hash => hash.ToString() == CurrentTokenHashValue)
                );
                _refreshTokenConsumer.Received(1).Consume(
                    Arg.Is<StoredRefreshToken>(stored => stored.GetConsumedAt() == Now)
                );
                _accessTokenIssuer.Received(1).Issue(
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
                _refreshTokenSaver.Received(1).Save(
                    Arg.Is<RefreshToken>(token => token.ToString() == NewRefreshTokenValue),
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
                _refreshTokenRevoker.DidNotReceive().Revoke(Arg.Any<StoredRefreshToken>());
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RevokeTheSessionWhenTheRefreshTokenWasAlreadyUsed()
    {
        _refreshTokenFinder.Find(Arg.Any<RefreshTokenHash>())
            .Returns(StoredRefreshTokenBuilder.Build(Now.AddDays(1), consumedAt: Now.AddMinutes(-5), owner: Username));

        var result = await _refresher.Execute(RefreshInfoBuilder.Build());

        result.Match(
            tokens => tokens.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<RefreshTokenReuseException>();
                _refreshTokenRevoker.Received(1).Revoke(
                    Arg.Is<StoredRefreshToken>(stored => stored.GetOwner() == Username)
                );
                _accessTokenIssuer.DidNotReceive().Issue(Arg.Any<AdminUsername>());
                _refreshTokenSaver.DidNotReceive().Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>());
            }
        );
    }

    [Fact]
    public async Task RevokeTheSessionWhenAnotherRequestConsumedTheRefreshTokenFirst()
    {
        _refreshTokenConsumer.Consume(Arg.Any<StoredRefreshToken>())
            .Returns(Error.New(new RefreshTokenReuseException("Refresh token has already been used")));

        var result = await _refresher.Execute(RefreshInfoBuilder.Build());

        result.Match(
            tokens => tokens.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<RefreshTokenReuseException>();
                _refreshTokenRevoker.Received(1).Revoke(Arg.Any<StoredRefreshToken>());
            }
        );
    }

    [Fact]
    public async Task KeepTheSessionWhenTheRefreshTokenHasExpired()
    {
        _refreshTokenFinder.Find(Arg.Any<RefreshTokenHash>())
            .Returns(StoredRefreshTokenBuilder.Build(Now.AddSeconds(-1), owner: Username));

        var result = await _refresher.Execute(RefreshInfoBuilder.Build());

        result.Match(
            tokens => tokens.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<UnauthorizedException>();
                _refreshTokenRevoker.DidNotReceive().Revoke(Arg.Any<StoredRefreshToken>());
            }
        );
    }
}
