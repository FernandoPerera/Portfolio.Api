using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Integration;

public class AuthenticationShould(PortfolioApiFactory factory) : IClassFixture<PortfolioApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    public static TheoryData<string, object> WriteEndpoints => new()
    {
        { "v1/language", new { languageCode = "es", name = "Spanish" } },
        { "v1/timeline", new { organization = "Lean Mind" } },
        { "v1/profile/create", new { name = "Fernando" } }
    };

    public static TheoryData<string> PublicEndpoints => new()
    {
        "v1/profile",
        "v1/timeline"
    };

    [Theory]
    [MemberData(nameof(WriteEndpoints))]
    public async Task RejectWritesWithoutToken(string url, object body)
    {
        var response = await _client.PostAsJsonAsync(url, body);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("application/problem+json");
    }

    [Theory]
    [MemberData(nameof(WriteEndpoints))]
    public async Task RejectWritesWithAnInvalidToken(string url, object body)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-valid-token");

        var response = await _client.PostAsJsonAsync(url, body);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [MemberData(nameof(PublicEndpoints))]
    public async Task AllowReadsWithoutToken(string url)
    {
        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AllowWritesWithAValidToken()
    {
        var tokens = await Login();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await _client.PostAsJsonAsync("v1/language", new { languageCode = "fr", name = "French" });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RejectLoginWithWrongCredentials()
    {
        var response = await _client.PostAsJsonAsync("v1/auth/login", new { username = "admin", password = "wrong" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RotateTheRefreshToken()
    {
        var tokens = await Login();

        var response = await _client.PostAsJsonAsync("v1/auth/refresh", new { refreshToken = tokens.RefreshToken });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var refreshed = (await response.Content.ReadFromJsonAsync<TokensResponse>())!;
        refreshed.RefreshToken.ShouldNotBe(tokens.RefreshToken);
        refreshed.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RevokeTheSessionWhenARefreshTokenIsReused()
    {
        var tokens = await Login();
        var rotated = await _client.PostAsJsonAsync("v1/auth/refresh", new { refreshToken = tokens.RefreshToken });
        var latest = (await rotated.Content.ReadFromJsonAsync<TokensResponse>())!;

        var reuse = await _client.PostAsJsonAsync("v1/auth/refresh", new { refreshToken = tokens.RefreshToken });
        var afterReuse = await _client.PostAsJsonAsync("v1/auth/refresh", new { refreshToken = latest.RefreshToken });

        reuse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        afterReuse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task<TokensResponse> Login()
    {
        var response = await _client.PostAsJsonAsync(
            "v1/auth/login",
            new { username = PortfolioApiFactory.AdminUsername, password = PortfolioApiFactory.AdminPassword }
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        return (await response.Content.ReadFromJsonAsync<TokensResponse>())!;
    }
}
