using System.Net.Http.Headers;
using System.Text.Json;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class GitHubIdentityProvider(IHttpClientFactory httpClientFactory, IOptions<GitHubOptions> gitHubOptions) : IExternalIdentityProvider
{
    public const string ClientName = "github";

    public async Task<Either<Error, ExternalUserId>> Identify(AuthorizationCode code)
    {
        try
        {
            var accessToken = await ExchangeCode(code);
            if (accessToken is null)
            {
                return Error.New(new UnauthorizedException("The authorization code was rejected by GitHub"));
            }

            return await FindUserId(accessToken);
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("GitHub is unavailable, error: " + error.Message));
        }
    }

    private async Task<string?> ExchangeCode(AuthorizationCode code)
    {
        var options = gitHubOptions.Value;
        using var request = new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = options.ClientId,
                ["client_secret"] = options.ClientSecret,
                ["code"] = code.ToString(),
                ["redirect_uri"] = options.RedirectUri
            })
        };
        request.Headers.UserAgent.ParseAdd(options.UserAgent);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var httpClient = httpClientFactory.CreateClient(ClientName);
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        return body.RootElement.TryGetProperty("access_token", out var accessToken)
            ? accessToken.GetString()
            : null;
    }

    private async Task<Either<Error, ExternalUserId>> FindUserId(string accessToken)
    {
        var options = gitHubOptions.Value;
        using var request = new HttpRequestMessage(HttpMethod.Get, options.UserEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd(options.UserAgent);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        using var httpClient = httpClientFactory.CreateClient(ClientName);
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        return ExternalUserId.Create(body.RootElement.GetProperty("id").GetInt64());
    }
}
