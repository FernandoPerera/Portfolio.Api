using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Portfolio.Api.Shared.Infrastructure.Options;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class RateLimitingExtensions
{
    public const string AuthPolicy = "auth";
    public const string PublicPolicy = "public";

    private const string UnknownClient = "unknown";

    public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRateLimiter(_ => { });
        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitingOptions>>((limiter, rateLimiting) =>
            {
                limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                limiter.OnRejected = WriteTooManyRequests;
                limiter.AddPolicy(AuthPolicy, context => PartitionByClient(context, rateLimiting.Value.Auth));
                limiter.AddPolicy(PublicPolicy, context => PartitionByClient(context, rateLimiting.Value.Public));
            });
    }

    private static RateLimitPartition<string> PartitionByClient(HttpContext context, RateLimitPolicyOptions policy)
    {
        return RateLimitPartition.GetFixedWindowLimiter(GetClient(context), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = policy.PermitLimit,
            Window = TimeSpan.FromSeconds(policy.WindowSeconds),
            QueueLimit = 0
        });
    }

    private static string GetClient(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? UnknownClient;

    private static async ValueTask WriteTooManyRequests(OnRejectedContext context, CancellationToken cancellationToken)
    {
        context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(RateLimitingExtensions).FullName!)
            .LogWarning(
                "Rate limit exceeded by {Client} on {Path}",
                GetClient(context.HttpContext),
                context.HttpContext.Request.Path
            );

        var response = context.HttpContext.Response;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString();
        }

        await response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too Many Requests",
                Type = "https://http.cat/429",
                Detail = "Rate limit exceeded. Try again later.",
                Instance = context.HttpContext.Request.Path
            },
            options: null,
            contentType: "application/problem+json",
            cancellationToken
        );
    }
}
