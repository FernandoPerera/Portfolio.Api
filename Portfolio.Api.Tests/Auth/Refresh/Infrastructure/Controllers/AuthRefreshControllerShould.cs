using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Infrastructure.Controllers;
using Portfolio.Api.Auth.Refresh.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Refresh.Infrastructure.Controllers;

public class AuthRefreshControllerShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 2, 10, 0, 0, TimeSpan.Zero);

    private readonly AppDbContext _appDbContext;
    private readonly AuthRefreshController _controller;

    public AuthRefreshControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var jwtOptions = new JwtOptions
        {
            Issuer = "portfolio-api",
            Audience = "portfolio-clients",
            SigningKey = "a-signing-key-with-at-least-32-bytes!"
        };
        var timeProvider = new FixedTimeProvider(Now);

        _appDbContext = new AppDbContext(options);
        var hasher = new Sha256RefreshTokenHasher();
        var refresher = new TokenRefresher(
            hasher,
            new EntityFrameworkRefreshTokenFinderAdapter(_appDbContext),
            new EntityFrameworkRefreshTokenConsumerAdapter(_appDbContext),
            new EntityFrameworkRefreshTokenRevokerAdapter(_appDbContext),
            new JwtAccessTokenIssuer(jwtOptions, timeProvider),
            new RandomRefreshTokenIssuer(),
            new EntityFrameworkRefreshTokenSaverAdapter(
                _appDbContext,
                new GuidGeneratorIdentifier(),
                hasher,
                jwtOptions,
                timeProvider
            ),
            new EntityFrameworkExpiredRefreshTokenCleanerAdapter(_appDbContext, timeProvider),
            timeProvider
        );
        _controller = new AuthRefreshController(refresher, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task RefreshTheTokens()
    {
        var current = RefreshTokenBuilder.Build("current-refresh-token");
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(current, expiresAt: Now.AddDays(1)));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Refresh(new RefreshRequest(current.ToString()));

        var tokens = response.Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<TokensResponse>();
        tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokens.RefreshToken.ShouldNotBe(current.ToString());
        _appDbContext.RefreshTokens.Count().ShouldBe(2);
    }

    [Fact]
    public async Task FailWhenTheRefreshTokenIsUnknown()
    {
        var response = await _controller.Refresh(new RefreshRequest("unknown"));

        var objectResult = response.Result.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status401Unauthorized);
    }
}
