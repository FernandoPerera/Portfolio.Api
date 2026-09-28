using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Shared.Infrastructure.Extensions;

public static class SharedInfrastructureExtensions
{
    public static void AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IGeneratorIdentifier, GuidGeneratorIdentifier>();
        services.AddScoped<ILanguageFinderRepository, EntityFrameworkLanguageFinderAdapter>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("PortfolioDb");
        });
    }
}
