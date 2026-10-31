using System.Net;
using System.Text;
using NSubstitute;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class GitHubIdentityProviderShould
{
    private const string TokenUrl = "https://github.test/login/oauth/access_token";
    private const string UserUrl = "https://api.github.test/user";
    private const string AccessToken = "gho_access_token";

    private readonly GitHubOptions _options = new()
    {
        ClientId = "client-id",
        ClientSecret = "client-secret",
        RedirectUri = "https://portfolio.test/callback",
        TokenEndpoint = TokenUrl,
        UserEndpoint = UserUrl,
        UserAgent = "Portfolio.Api.Tests"
    };

    [Fact]
    public async Task IdentifyTheUserWithTheirGitHubId()
    {
        const string code = "github-authorization-code";
        var handler = new StubHttpMessageHandler((request, _) => request.RequestUri!.ToString() switch
        {
            TokenUrl => Json($$"""{"access_token":"{{AccessToken}}","token_type":"bearer"}"""),
            _ => Json("""{"id":123456,"login":"someone"}""")
        });
        var provider = CreateProvider(handler);

        var result = await provider.Identify(AuthorizationCodeBuilder.Build(code));

        result.Match(
            userId =>
            {
                userId.ToString().ShouldBe("123456");
                AssertCodeWasExchanged(handler.Requests[0], code);
                AssertUserWasRequested(handler.Requests[1].Request);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RejectTheCodeWhenGitHubDoesNotAcceptIt()
    {
        var handler = new StubHttpMessageHandler((_, _) => Json("""{"error":"bad_verification_code"}"""));
        var provider = CreateProvider(handler);

        var result = await provider.Identify(AuthorizationCodeBuilder.Build());

        result.Match(
            userId => userId.ShouldBeNull(),
            error =>
            {
                error.ShouldBeOfType<UnauthorizedError>();
                handler.Requests.Count.ShouldBe(1);
            }
        );
    }

    [Fact]
    public async Task FailWhenGitHubIsUnavailable()
    {
        var handler = new StubHttpMessageHandler((_, _) => throw new HttpRequestException("GitHub is down"));
        var provider = CreateProvider(handler);

        var result = await provider.Identify(AuthorizationCodeBuilder.Build());

        result.Match(
            userId => userId.ShouldBeNull(),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    [Fact]
    public async Task FailWhenGitHubRespondsWithAnError()
    {
        var handler = new StubHttpMessageHandler((request, _) => request.RequestUri!.ToString() switch
        {
            TokenUrl => Json($$"""{"access_token":"{{AccessToken}}"}"""),
            _ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        });
        var provider = CreateProvider(handler);

        var result = await provider.Identify(AuthorizationCodeBuilder.Build());

        result.Match(
            userId => userId.ShouldBeNull(),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private GitHubIdentityProvider CreateProvider(StubHttpMessageHandler handler)
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient(GitHubIdentityProvider.ClientName).Returns(_ => new HttpClient(handler));

        return new GitHubIdentityProvider(httpClientFactory, _options);
    }

    private void AssertCodeWasExchanged((HttpRequestMessage Request, string Body) tokenRequest, string code)
    {
        tokenRequest.Request.RequestUri!.ToString().ShouldBe(_options.TokenEndpoint);
        tokenRequest.Request.Headers.UserAgent.ToString().ShouldBe(_options.UserAgent);
        tokenRequest.Body.ShouldContain($"client_id={_options.ClientId}");
        tokenRequest.Body.ShouldContain($"client_secret={_options.ClientSecret}");
        tokenRequest.Body.ShouldContain($"code={code}");
        tokenRequest.Body.ShouldContain($"redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}");
    }

    private void AssertUserWasRequested(HttpRequestMessage userRequest)
    {
        userRequest.RequestUri!.ToString().ShouldBe(_options.UserEndpoint);
        userRequest.Headers.Authorization!.ToString().ShouldBe($"Bearer {AccessToken}");
        userRequest.Headers.UserAgent.ToString().ShouldBe(_options.UserAgent);
    }

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
}
