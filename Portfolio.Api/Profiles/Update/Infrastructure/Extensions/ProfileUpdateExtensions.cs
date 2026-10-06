using Portfolio.Api.Profiles.Update.Application.UseCases;
using Portfolio.Api.Profiles.Update.Domain.Ports;
using Portfolio.Api.Profiles.Update.Infrastructure.Adapters;

namespace Portfolio.Api.Profiles.Update.Infrastructure.Extensions;

public static class ProfileUpdateExtensions
{
    public static void AddProfileUpdate(this IServiceCollection services)
    {
        services.AddScoped<ProfileUpdater>();
        services.AddScoped<IProfileUpdaterRepository, EntityFrameworkProfileUpdaterAdapter>();
    }
}
