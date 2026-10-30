using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Logout.Application.UseCases;
using Portfolio.Api.Auth.Logout.Infrastructure.Controllers;
using Portfolio.Api.Auth.Logout.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Auth.Shared.Infrastructure.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Logout.Infrastructure.Controllers;

public class AuthLogoutControllerShould
{
    private const string OwnerRefreshTokenValue = "owner-refresh-token";
    private const string OtherOwnerRefreshTokenValue = "other-owner-refresh-token";
    private const string Owner = "admin";
    private const string OtherOwner = "someone-else";

    private readonly AppDbContext _appDbContext;
    private readonly AuthLogoutController _controller;

    public AuthLogoutControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var closer = new SessionCloser(
            new Sha256RefreshTokenHasher(),
            new EntityFrameworkRefreshTokenFinderAdapter(_appDbContext),
            new EntityFrameworkRefreshTokenRevokerAdapter(_appDbContext)
        );
        _controller = new AuthLogoutController(closer, new ProblemDetailsMapper());
    }

    [Fact]
    public async Task CloseTheSessionOfTheOwner()
    {
        var consumed = RefreshTokenBuilder.Build("consumed-refresh-token");
        var current = RefreshTokenBuilder.Build(OwnerRefreshTokenValue);
        var otherOwner = RefreshTokenBuilder.Build(OtherOwnerRefreshTokenValue);
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(consumed, Owner, consumedAt: DateTimeOffset.UtcNow));
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(current, Owner));
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(otherOwner, OtherOwner));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Logout(new LogoutRequest(current.ToString()));

        response.ShouldBeOfType<NoContentResult>();
        _appDbContext.RefreshTokens.Select(token => token.Owner).ToList().ShouldBe([OtherOwner]);
    }

    [Fact]
    public async Task CloseNothingWhenTheRefreshTokenIsUnknown()
    {
        var other = RefreshTokenBuilder.Build(OtherOwnerRefreshTokenValue);
        _appDbContext.RefreshTokens.Add(RefreshTokenEntityBuilder.Build(other, OtherOwner));
        await _appDbContext.SaveChangesAsync();

        var response = await _controller.Logout(new LogoutRequest("unknown"));

        response.ShouldBeOfType<NoContentResult>();
        _appDbContext.RefreshTokens.Count().ShouldBe(1);
    }
}
