using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Controllers;
using Portfolio.Api.Auth.Login.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Infrastructure.Controllers;

public class AuthLoginControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly AuthLoginController _controller;

    public AuthLoginControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "portfolio-api",
            Audience = "portfolio-clients",
            SigningKey = "a-signing-key-with-at-least-32-bytes!"
        });
        var adminOptions = Options.Create(new AdminOptions
        {
            Username = "admin",
            PasswordHash = Pbkdf2PasswordHasher.Hash("secret")
        });

        _appDbContext = new AppDbContext(options);
        var authenticator = new Authenticator(
            new ConfigurationCredentialsVerifier(adminOptions),
            new JwtAccessTokenIssuer(jwtOptions, TimeProvider.System),
            new RandomRefreshTokenIssuer(),
            new EntityFrameworkRefreshTokenSaverAdapter(_appDbContext, new GuidGeneratorIdentifier(), jwtOptions, TimeProvider.System)
        );
        _controller = new AuthLoginController(authenticator, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task LoginWithTheAdminCredentials()
    {
        var response = await _controller.Login(new LoginRequest("admin", "secret"));

        var tokens = response.Result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<TokensResponse>();
        tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokens.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        _appDbContext.RefreshTokens.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task FailWhenTheCredentialsAreWrong()
    {
        var response = await _controller.Login(new LoginRequest("admin", "wrong"));

        var objectResult = response.Result.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task FailWhenTheCredentialsAreMissing()
    {
        var response = await _controller.Login(new LoginRequest("", ""));

        var objectResult = response.Result.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
}
