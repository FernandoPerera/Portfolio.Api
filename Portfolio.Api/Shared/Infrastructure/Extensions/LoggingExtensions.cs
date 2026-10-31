using Microsoft.AspNetCore.HttpLogging;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class LoggingExtensions
{
    private const string HealthPath = "/health";

    public static void AddRequestLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestMethod
                | HttpLoggingFields.RequestPath
                | HttpLoggingFields.ResponseStatusCode
                | HttpLoggingFields.Duration;
            options.CombineLogs = true;
        });
    }

    public static void UseRequestLogging(this WebApplication app)
    {
        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments(HealthPath),
            branch => branch.UseHttpLogging()
        );
    }
}
