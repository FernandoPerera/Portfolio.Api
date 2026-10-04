using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Refresh.Application.Builders;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Refresh.Application.UseCases;

public class TokenRefresherShould
{
    private const string Username = "admin";
    private const string CurrentRefreshTokenValue = "current-refresh-token";
    private const string NewAccessTokenValue = "new-access-token";
    private const string NewRefreshTokenValue = "new-refresh-token";

    private readonly IRefreshTokenConsumerRepository _refreshTokenConsumer = Substitute.For<IRefreshTokenConsumerRepository>();
    private readonly IAccessTokenIssuer _accessTokenIssuer = Substitute.For<IAccessTokenIssuer>();
    private readonly IRefreshTokenIssuer _refreshTokenIssuer = Substitute.For<IRefreshTokenIssuer>();
    private readonly IRefreshTokenSaverRepository _refreshTokenSaver = Substitute.For<IRefreshTokenSaverRepository>();
    private readonly TokenRefresher _refresher;

    public TokenRefresherShould()
    {
        _refresher = new TokenRefresher(_refreshTokenConsumer, _accessTokenIssuer, _refreshTokenIssuer, _refreshTokenSaver);
        _refreshTokenConsumer.Consume(Arg.Any<RefreshToken>()).Returns(AdminUsernameBuilder.Build(Username));
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
                _refreshTokenConsumer.Received(1).Consume(
                    Arg.Is<RefreshToken>(token => token.ToString() == CurrentRefreshTokenValue)
                );
                _accessTokenIssuer.Received(1).Issue(
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
                _refreshTokenSaver.Received(1).Save(
                    Arg.Is<RefreshToken>(token => token.ToString() == NewRefreshTokenValue),
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotIssueTokensWhenTheRefreshTokenIsRejected()
    {
        _refreshTokenConsumer.Consume(Arg.Any<RefreshToken>())
            .Returns(Error.New(new UnauthorizedException("Invalid refresh token")));

        var result = await _refresher.Execute(RefreshInfoBuilder.Build());

        result.Match(
            tokens => tokens.ShouldBeNull(),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<UnauthorizedException>();
                _accessTokenIssuer.DidNotReceive().Issue(Arg.Any<AdminUsername>());
                _refreshTokenSaver.DidNotReceive().Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>());
            }
        );
    }
}
