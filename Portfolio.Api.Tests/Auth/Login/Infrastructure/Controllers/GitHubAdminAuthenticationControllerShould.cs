using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Cookies;
using Portfolio.Api.Auth.Login.Infrastructure.Controllers;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Infrastructure.Controllers;

public class GitHubAdminAuthenticationControllerShould
{
    private const string FrontendRedirectUri = "https://portfolio.test/login";
    private const string State = "oauth-state";

    private readonly IExternalIdentityProvider _identityProvider = Substitute.For<IExternalIdentityProvider>();
    private readonly IAdminAuthorizer _adminAuthorizer = Substitute.For<IAdminAuthorizer>();
    private readonly ILoginCodeIssuer _loginCodeIssuer = Substitute.For<ILoginCodeIssuer>();
    private readonly ILoginCodeSaverRepository _loginCodeSaver = Substitute.For<ILoginCodeSaverRepository>();
    private readonly GitHubAdminAuthenticationController _controller;

    public GitHubAdminAuthenticationControllerShould()
    {
        var authenticator = new GitHubAdminAuthenticator(_identityProvider, _adminAuthorizer, _loginCodeIssuer, _loginCodeSaver);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = $"{OAuthStateCookie.Name}={State}";
        _controller = new GitHubAdminAuthenticationController(
            authenticator,
            new AuthOptions { FrontendRedirectUri = FrontendRedirectUri },
            new ProblemDetailsMapper()
        )
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
        _identityProvider.Identify(Arg.Any<AuthorizationCode>()).Returns(ExternalUserIdBuilder.Build());
        _adminAuthorizer.EnsureIsAdmin(Arg.Any<ExternalUserId>()).Returns(Unit.Default);
        _loginCodeIssuer.Issue().Returns(LoginCodeBuilder.Build("login-code"));
        _loginCodeSaver.Save(Arg.Any<LoginCode>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task RedirectToTheWebWithTheLoginCode()
    {
        var response = await _controller.Callback("github-code", State);

        var redirect = response.ShouldBeOfType<RedirectResult>();
        redirect.Url.ShouldStartWith(FrontendRedirectUri);
        QueryHelpers.ParseQuery(new Uri(redirect.Url).Query)["code"].ToString().ShouldBe("login-code");
        _controller.Response.Headers.SetCookie.ToString().ShouldContain($"{OAuthStateCookie.Name}=;");
    }

    [Fact]
    public async Task RejectTheCallbackWhenTheStateIsNotTheOneInTheCookie()
    {
        var response = await _controller.Callback("github-code", "forged-state");

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        objectResult.Value.ShouldBeOfType<ProblemDetails>().Status.ShouldBe(StatusCodes.Status401Unauthorized);
        _loginCodeSaver.DidNotReceive().Save(Arg.Any<LoginCode>());
    }
}
