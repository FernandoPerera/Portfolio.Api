using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Login.Application.UseCases;
using Portfolio.Api.Auth.Login.Infrastructure.Cookies;
using Portfolio.Api.Auth.Login.Infrastructure.Controllers;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Login.Infrastructure.Controllers;

public class GitHubLoginControllerShould
{
    private readonly GitHubOptions _options = new()
    {
        ClientId = "client-id",
        RedirectUri = "https://portfolio.test/v1/auth/github/callback",
        AuthorizeEndpoint = "https://github.test/login/oauth/authorize"
    };

    [Fact]
    public void RedirectToGitHubAndRememberTheState()
    {
        var controller = CreateController(_options);

        var response = controller.Login();

        var redirect = response.ShouldBeOfType<RedirectResult>();
        var state = QueryHelpers.ParseQuery(new Uri(redirect.Url).Query)["state"].ToString();
        var cookie = controller.Response.Headers.SetCookie.ToString();
        cookie.ShouldContain($"{OAuthStateCookie.Name}={state}");
        cookie.ShouldContain("httponly");
        cookie.ShouldContain("samesite=lax");
        cookie.ShouldContain($"path={OAuthStateCookie.Path}");
    }

    private static GitHubLoginController CreateController(GitHubOptions options)
    {
        var starter = new GitHubLoginStarter(
            new RandomOAuthStateIssuer(),
            new GitHubAuthorizationUrlBuilder(options)
        );

        return new GitHubLoginController(starter, new ProblemDetailsMapper())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }
}
