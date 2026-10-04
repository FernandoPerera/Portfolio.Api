using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ProblemDetailsJwtBearerEvents : JwtBearerEvents
{
    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://http.cat/401",
                Detail = "A valid access token is required",
                Instance = context.Request.Path
            },
            options: null,
            contentType: "application/problem+json"
        );
    }
}
