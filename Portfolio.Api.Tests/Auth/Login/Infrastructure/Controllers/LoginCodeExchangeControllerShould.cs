using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Controllers;
using Portfolio.Api.Auth.Login.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Infrastructure.Controllers;

public class LoginCodeExchangeControllerShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly IRefreshTokenSaverRepository _refreshTokenSaver = Substitute.For<IRefreshTokenSaverRepository>();
    private readonly IExpiredRefreshTokenCleanerRepository _expiredRefreshTokenCleaner = Substitute.For<IExpiredRefreshTokenCleanerRepository>();
    private readonly LoginCodeExchangeController _controller;

    public LoginCodeExchangeControllerShould()
    {
        var timeProvider = new FixedTimeProvider(Now);
        var jwtOptions = new JwtOptions
        {
            Issuer = "portfolio-api",
            Audience = "portfolio-clients",
            SigningKey = "a-signing-key-with-at-least-32-bytes!"
        };
        var exchanger = new LoginCodeExchanger(
            new InMemoryLoginCodeConsumerAdapter(_cache, timeProvider),
            new ConfigurationAdminUsernameProvider(new AdminOptions { Username = "admin" }),
            new JwtAccessTokenIssuer(jwtOptions, timeProvider),
            new RandomRefreshTokenIssuer(),
            _refreshTokenSaver,
            _expiredRefreshTokenCleaner
        );
        _controller = new LoginCodeExchangeController(exchanger, new ProblemDetailsMapper());
        _refreshTokenSaver.Save(Arg.Any<RefreshToken>(), Arg.Any<AdminUsername>()).Returns(LanguageExt.Unit.Default);
        _expiredRefreshTokenCleaner.Clean().Returns(LanguageExt.Unit.Default);
    }

    [Fact]
    public async Task ExchangeTheLoginCodeForTokens()
    {
        var loginCode = LoginCodeBuilder.Build("login-code");
        new InMemoryLoginCodeSaverAdapter(_cache, new FixedTimeProvider(Now)).Save(loginCode);

        var response = await _controller.Exchange(new ExchangeRequest(loginCode.ToString()));

        var tokens = response.Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<TokensResponse>();
        tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokens.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task FailWhenTheLoginCodeIsUnknown()
    {
        var response = await _controller.Exchange(new ExchangeRequest("unknown"));

        var objectResult = response.Result.ShouldBeOfType<ObjectResult>();
        objectResult.Value.ShouldBeOfType<ProblemDetails>().Status.ShouldBe(StatusCodes.Status401Unauthorized);
    }
}
