using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class GitHubAuthorizationUrlBuilder(IOptions<GitHubOptions> gitHubOptions) : IAuthorizationUrlBuilder
{
    public Uri Build(OAuthState state)
    {
        var options = gitHubOptions.Value;
        var url = QueryHelpers.AddQueryString(options.AuthorizeEndpoint, new Dictionary<string, string?>
        {
            ["client_id"] = options.ClientId,
            ["redirect_uri"] = options.RedirectUri,
            ["state"] = state.ToString()
        });

        return new Uri(url);
    }
}
