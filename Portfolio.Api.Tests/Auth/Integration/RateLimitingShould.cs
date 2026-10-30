using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Integration;

public class RateLimitingShould
{
    [Fact]
    public async Task RejectTooManyRequestsToTheAuthEndpoints()
    {
        using var factory = new RateLimitedApiFactory();
        var client = factory.CreateClient();

        var responses = await SendRequests(
            RateLimitedApiFactory.PermitLimit + 1,
            () => client.PostAsJsonAsync("v1/auth/exchange", new { code = "unknown" })
        );

        responses[..RateLimitedApiFactory.PermitLimit].ShouldAllBe(status => status == HttpStatusCode.Unauthorized);
        responses[^1].ShouldBe(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task RejectTooManyRequestsToThePublicEndpoints()
    {
        using var factory = new RateLimitedApiFactory();
        var client = factory.CreateClient();

        var responses = await SendRequests(
            RateLimitedApiFactory.PermitLimit + 1,
            () => client.GetAsync("v1/timeline")
        );

        responses[..RateLimitedApiFactory.PermitLimit].ShouldAllBe(status => status == HttpStatusCode.OK);
        responses[^1].ShouldBe(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task AnswerTooManyRequestsAsProblemDetails()
    {
        using var factory = new RateLimitedApiFactory();
        var client = factory.CreateClient();
        await SendRequests(RateLimitedApiFactory.PermitLimit, () => client.GetAsync("v1/profile"));

        var response = await client.GetAsync("v1/profile");

        response.Content.Headers.ContentType!.MediaType.ShouldBe("application/problem+json");
        response.Headers.RetryAfter.ShouldNotBeNull();
    }

    private static async Task<List<HttpStatusCode>> SendRequests(int count, Func<Task<HttpResponseMessage>> send)
    {
        var statuses = new List<HttpStatusCode>();
        for (var i = 0; i < count; i++)
        {
            statuses.Add((await send()).StatusCode);
        }

        return statuses;
    }
}
