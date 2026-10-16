using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.Dtos;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Application.UseCases;

public class LoginCodeExchangerShould
{
    private const string Username = "admin";
    private const string LoginCodeValue = "login-code";
    private const string AccessTokenValue = "access-token";
    private const string RefreshTokenValue = "refresh-token";

    private readonly ILoginCodeConsumerRepository _loginCodeConsumer = Substitute.For<ILoginCodeConsumerRepository>();
    private readonly IAdminUsernameProvider _adminUsernameProvider = Substitute.For<IAdminUsernameProvider>();
    private readonly IAccessTokenIssuer _accessTokenIssuer = Substitute.For<IAccessTokenIssuer>();
    private readonly IRefreshTokenIssuer _refreshTokenIssuer = Substitute.For<IRefreshTokenIssuer>();
    private readonly IRefreshTokenSaverRepository _refreshTokenSaver = Substitute.For<IRefreshTokenSaverRepository>();
    private readonly IExpiredRefreshTokenCleanerRepository _expiredRefreshTokenCleaner = Substitute.For<IExpiredRefreshTokenCleanerRepository>();
    private readonly LoginCodeExchanger _exchanger;

    public LoginCodeExchangerShould()
    {
        _exchanger = new LoginCodeExchanger(
            _loginCodeConsumer,
            _adminUsernameProvider,
            _accessTokenIssuer,
            _refreshTokenIssuer,
            _refreshTokenSaver,
            _expiredRefreshTokenCleaner
        );
        _loginCodeConsumer.Consume(Arg.Any<LoginCode>()).Returns(Unit.Default);
        _adminUsernameProvider.Get().Returns(AdminUsernameBuilder.Build(Username));
        _accessTokenIssuer.Issue(Arg.Any<AdminUsername>()).Returns(AccessTokenBuilder.Build(AccessTokenValue));
        _refreshTokenIssuer.Issue().Returns(RefreshTokenBuilder.Build(RefreshTokenValue));
        _refreshTokenSaver.Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>()).Returns(Unit.Default);
        _expiredRefreshTokenCleaner.Clean().Returns(Unit.Default);
    }

    [Fact]
    public async Task ExchangeTheLoginCodeForTokens()
    {
        var result = await _exchanger.Execute(new ExchangeInfo(LoginCodeValue));

        result.Match(
            tokens =>
            {
                tokens.GetAccessToken().ShouldBe(AccessTokenValue);
                tokens.GetRefreshToken().ShouldBe(RefreshTokenValue);
                _loginCodeConsumer.Received(1).Consume(Arg.Is<LoginCode>(code => code.ToString() == LoginCodeValue));
                _accessTokenIssuer.Received(1).Issue(Arg.Is<AdminUsername>(owner => owner.ToString() == Username));
                _refreshTokenSaver.Received(1).Save(
                    Arg.Is<RefreshToken>(token => token.ToString() == RefreshTokenValue),
                    Arg.Is<AdminUsername>(owner => owner.ToString() == Username)
                );
                _expiredRefreshTokenCleaner.Received(1).Clean();
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotIssueTokensWhenTheLoginCodeIsRejected()
    {
        _loginCodeConsumer.Consume(Arg.Any<LoginCode>())
            .Returns(Error.New(new UnauthorizedException("Invalid login code")));

        var result = await _exchanger.Execute(new ExchangeInfo(LoginCodeValue));

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
