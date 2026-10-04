using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Login.Application.Builders;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Application.UseCases;

public class AuthenticatorShould
{
    private const string Username = "admin";
    private const string AccessTokenValue = "access-token";
    private const string RefreshTokenValue = "refresh-token";

    private readonly ICredentialsVerifier _credentialsVerifier = Substitute.For<ICredentialsVerifier>();
    private readonly IAccessTokenIssuer _accessTokenIssuer = Substitute.For<IAccessTokenIssuer>();
    private readonly IRefreshTokenIssuer _refreshTokenIssuer = Substitute.For<IRefreshTokenIssuer>();
    private readonly IRefreshTokenSaverRepository _refreshTokenSaver = Substitute.For<IRefreshTokenSaverRepository>();
    private readonly Authenticator _authenticator;

    public AuthenticatorShould()
    {
        _authenticator = new Authenticator(_credentialsVerifier, _accessTokenIssuer, _refreshTokenIssuer, _refreshTokenSaver);
        _credentialsVerifier.Verify(Arg.Any<AdminCredentials>()).Returns(Unit.Default);
        _accessTokenIssuer.Issue(Arg.Any<AdminUsername>()).Returns(AccessTokenBuilder.Build(AccessTokenValue));
        _refreshTokenIssuer.Issue().Returns(RefreshTokenBuilder.Build(RefreshTokenValue));
        _refreshTokenSaver.Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task LoginAndIssueTokens()
    {
        var result = await _authenticator.Execute(LoginInfoBuilder.Build(Username));

        result.Match(
            tokens =>
            {
                tokens.GetAccessToken().ShouldBe(AccessTokenValue);
                tokens.GetRefreshToken().ShouldBe(RefreshTokenValue);
                _refreshTokenSaver.Received(1).Save(
                    Arg.Is<RefreshToken>(token => token.ToString() == RefreshTokenValue),
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotIssueTokensWhenCredentialsAreWrong()
    {
        _credentialsVerifier.Verify(Arg.Any<AdminCredentials>())
            .Returns(Error.New(new UnauthorizedException("Invalid credentials")));

        var result = await _authenticator.Execute(LoginInfoBuilder.Build(Username));

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
