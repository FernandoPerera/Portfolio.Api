using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Profiles.Find.Infrastructure.Adapters;

namespace Portfolio.Api.Profiles.Find.Infrastructure.Extensions;

public static class ProfileFindExtensions
{
    public static void AddProfileFind(this IServiceCollection services)
    {
        services.AddScoped<ProfileFinder>();
        services.AddScoped<IProfileFinderRepository, EntityFrameworkProfileFinderAdapter>();
    }
}
