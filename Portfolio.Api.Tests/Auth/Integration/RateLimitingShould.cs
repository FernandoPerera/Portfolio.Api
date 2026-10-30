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
