using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class GitHubAuthorizationUrlBuilderShould
{
    private const string AuthorizeEndpoint = "https://github.test/login/oauth/authorize";

    private readonly GitHubOptions _options = new()
    {
        ClientId = "client-id",
        RedirectUri = "https://portfolio.test/v1/auth/github/callback",
        AuthorizeEndpoint = AuthorizeEndpoint
    };

    [Fact]
    public void BuildTheAuthorizationUrl()
    {
        const string state = "oauth-state";

        var url = CreateBuilder(_options).Build(OAuthStateBuilder.Build(state));

        var query = QueryHelpers.ParseQuery(url.Query);
        url.GetLeftPart(UriPartial.Path).ShouldBe(AuthorizeEndpoint);
        query["client_id"].ToString().ShouldBe(_options.ClientId);
        query["redirect_uri"].ToString().ShouldBe(_options.RedirectUri);
        query["state"].ToString().ShouldBe(state);
    }

    private static GitHubAuthorizationUrlBuilder CreateBuilder(GitHubOptions options) => new(options);
}
