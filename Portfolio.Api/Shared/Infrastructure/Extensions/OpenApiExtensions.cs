using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class OpenApiExtensions
{
    private const string BearerScheme = "Bearer";

    public static void AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(DescribeApi);
        });
    }

    public static void MapOpenApiDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference(options => options
            .WithTitle("Portfolio API")
            .AddPreferredSecuritySchemes(BearerScheme))
            .AllowAnonymous();
    }

    private static Task DescribeApi(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Portfolio API",
            Version = "v1",
            Description = """
                          API of a personal portfolio and technical blog.

                          **Visitors** can read the profile, the career timeline and the available languages without signing in.

                          **The owner** is the only one who can make changes. They sign in with GitHub, exchange the login code for an access token and send it as `Authorization: Bearer <access token>`. When the access token expires, a refresh token gives a new pair without signing in again.

                          Content is translated: every profile and timeline entry has one translation per language code.

                          Errors are returned as `ProblemDetails` (RFC 7807).
                          """
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[BearerScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Access token obtained when the owner signs in with GitHub."
        };

        return Task.CompletedTask;
    }
}
