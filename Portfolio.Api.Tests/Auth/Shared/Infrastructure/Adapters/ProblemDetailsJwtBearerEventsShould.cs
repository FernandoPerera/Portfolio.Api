using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class ProblemDetailsJwtBearerEventsShould
{
    [Fact]
    public async Task RespondUnauthorizedWithProblemDetails()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/v1/timeline";
        httpContext.Response.Body = new MemoryStream();
        var context = new JwtBearerChallengeContext(
            httpContext,
            new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler)),
            new JwtBearerOptions(),
            new AuthenticationProperties()
        );

        await new ProblemDetailsJwtBearerEvents().Challenge(context);

        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        httpContext.Response.ContentType.ShouldStartWith("application/problem+json");
        httpContext.Response.Body.Position = 0;
        var body = await JsonDocument.ParseAsync(httpContext.Response.Body);
        body.RootElement.GetProperty("status").GetInt32().ShouldBe(StatusCodes.Status401Unauthorized);
        body.RootElement.GetProperty("title").GetString().ShouldBe("Unauthorized");
        body.RootElement.GetProperty("instance").GetString().ShouldBe("/v1/timeline");
    }
}
