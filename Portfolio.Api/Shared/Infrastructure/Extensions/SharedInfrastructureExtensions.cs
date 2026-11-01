using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class SharedInfrastructureExtensions
{
    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IGeneratorIdentifier, GuidGeneratorIdentifier>();
        services.AddScoped<ILanguageExistenceRepository, EntityFrameworkLanguageExistenceAdapter>();
        services.AddSingleton<IErrorResponseMapper, ProblemDetailsMapper>();

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            options.UseNpgsql(provider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString);
        });
    }
}
