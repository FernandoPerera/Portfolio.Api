using Microsoft.AspNetCore.WebUtilities;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class GitHubAuthorizationUrlBuilder(GitHubOptions options) : IAuthorizationUrlBuilder
{
    public Uri Build(OAuthState state)
    {
        var url = QueryHelpers.AddQueryString(options.AuthorizeEndpoint, new Dictionary<string, string?>
        {
            ["client_id"] = options.ClientId,
            ["redirect_uri"] = options.RedirectUri,
            ["state"] = state.ToString()
        });

        return new Uri(url);
    }
}
