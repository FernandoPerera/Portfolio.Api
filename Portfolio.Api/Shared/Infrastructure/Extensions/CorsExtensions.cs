using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Portfolio.Api.Shared.Infrastructure.Options;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class CorsExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CorsPolicyOptions>().Bind(configuration.GetSection(CorsPolicyOptions.SectionName));

        services.AddCors();
        services.AddOptions<CorsOptions>()
            .Configure<IOptions<CorsPolicyOptions>>((cors, policy) =>
                cors.AddDefaultPolicy(builder => builder
                    .WithOrigins(policy.Value.AllowedOrigins)
                    .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Patch, HttpMethods.Delete)
                    .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
                    .WithExposedHeaders(HeaderNames.RetryAfter)));
    }
}
